<?php
/*
Plugin Name: PukiWikiLikeFormat
Plugin URI: http://nekoaruki.com/wordpress/pukiwikilikeformat
Description: PukiWiki風書式
Author: cat-walk
Version: 0.0.0
Author URI: http://nekoaruki.com
*/

define("CWPukiWikiLikeFormat_HeaderTitle", '見出し');
define("CWPukiWikiLikeFormat_AnchorChar", '&dagger;');

class CWPukiWikiLikeFormat{
	public static function init(){
		add_filter('the_excerpt', array(__CLASS__, 'postprocess'), 3);
		add_filter('the_content', array(__CLASS__, 'postprocess'), 3);
		add_filter('content_save_pre', array(__CLASS__, 'preprocess'));
	}
	
	public static function preprocess($content){
	}
	
	public static function postprocess($content){
		$parser = new CWPukiWikiLikeFormat_Parser();
		return $parser->parse($content);
	}
}

class CWPukiWikiLikeFormat_Parser{
	public $lines;
	public $current = 0;
	public $document;
	
	function CWPukiWikiLikeFormat_Parser(){
		$this->document = new CWPukiWikiLikeFormat_Document();
	}
	
	function getCurrentLine(){
		return $this->lines[$this->current];
	}
	
	function parse($content){
		$this->lines = explode("\r\n", $content);
		$count = count($this->lines);
		for(; $this->current < $count; $this->current++){
			$this->document->addChild("\r\n");
			$line = $this->lines[$this->current];
			if($line != ''){
				$offset = $line[0];
				switch($offset){
					// リスト
					case '-':{
						$list = new CWPukiWikiLikeFormat_ListElement(false);
						$list->addChild($this->extractInline(substr($line, 1)));
						$this->parseList($list, $offset);
						
						$this->document->addChild($list);
						break;
					}
					case '+':{
						$list = new CWPukiWikiLikeFormat_ListElement(true);
						$list->addChild($this->extractInline(substr($line, 1)));
						$this->parseList($list, $offset);
						
						$this->document->addChild($list);
						break;
					}
					// 定義リスト
					case ':':{
						$pos = strpos($line, '|');
						if($pos > 0){
							$list = new CWPukiWikiLikeFormat_Element('dl');
							$list->addChild('<dt>' . $this->extractInline(substr($line, 1, $pos - 1)) .
							                '</dt><dd>' . $this->extractInline(substr($line, $pos + 1)) . '</dd>');
							$this->parseDList($list);
							
							$this->document->addChild($list);
						}else{
							$this->document->addChild($this->extractInline($line));
						}
						break;
					}
					// 見出し
					case '*':{
						preg_match(
							'/^(\*+)(|\[[^\]]+\])(.+)$/m',
							$line,
							$matches);
						global $wp_query;
						$postID = $wp_query->post->ID;
						$level = 2 + strlen($matches[1]);
						$anchor = ($matches[2] != '') ?
						          substr($matches[2], 1, strlen($matches[2] - 2)) :
						          $postID . '_' . (count($this->document->headerList->items) + 1) . '_h' . $level;
						$content = $this->extractInline(rtrim($matches[3]));
						$this->document->headerList->items[] = new CWPukiWikiLikeFormat_HeaderListItem($content, '#' . $anchor, $level);
						$this->document->addChild('<h' . $level . '>' . $content .
						       '<a class="header-anchor" id="' . $anchor . '" href="#'. $anchor . '">' .
						       CWPukiWikiLikeFormat_AnchorChar . '</a></h' . $level . '>');
						break;
					}
					// 表組み
					case '|':{
						$elms = explode('|', $line);
						$elmsCount = count($elms);
						if($elmsCount > 2){
							$table = new CWPukiWikiLikeFormat_Table();
							$this->parseTableRow($table, $elms, $elmsCount);
							$this->parseTable($table);
							
							$this->document->addChild($table);
						}else{
							$this->document->addChild($this->extractInline($line));
						}
						break;
					}
					// ブロックプラグイン
					case '#':{
						$this->extractBlockPlugin($line);
						break;
					}
					default:{
						// コメント行
						if(($offset == '/') && (strpos($line, '//') === 0)){
							continue;
						// 通常
						}else{
							$this->document->addChild($this->extractInline($line));
						}
					}
				}
			}
		}
		return $this->document->toHtml();
	}

	function parseList($parent, $offset){
		$offsetlen = strlen($offset);
		$count = count($this->lines);
		for($this->current++; $this->current < $count; $this->current++){
			$line = $this->getCurrentLine();
			if(strpos($linel, $offset) === 0){
				if(strlen($lines[$current]) > $offsetlen){
					//$list = NULL;
					$c = $line[$offsetlen];
					switch($c){
						case '-':{
							$list = new CWPukiWikiLikeFormat_ListElement(false);
							break;
						}
						case '+':{
							$list = new CWPukiWikiLikeFormat_ListElement(true);
							break;
						}
					}
					if(!is_null($list)){
						$offset2 = $offset . $c;
						$parent->addChild($list);
						$list->addChild(substr($line, $offsetlen + 1));
						$this->parseList($list, $offset2);
					}else{
						$list->addChild(substr($lines[$current], $offsetlen));
					}
				}else{
					$list->addChild('');
				}
			}else{
				$this->current--;
				return;
			}
		}
	}

	function parseDList($list){
		$count = count($this->lines);
		for($this->current++; $this->current < $count; $this->current++){
			$line = $this->getCurrentLine();
			if(($line != '') && ($line[0] == ':')){
				$pos = strpos($line, '|');
				if($pos > 0){
					$list->addChild('<dt>' . $this->extractInline(substr($line, 1, $pos - 1)) .
					                '</dt><dd>' . $this->extractInline(substr($line, $pos + 1)) . '</dd>');
				}else{
					$this->current--;
					break;
				}
			}else{
				$this->current--;
				break;
			}
		}
	}
	
	function parseTable($table){
		$count = count($this->lines);
		for($this->current++; $this->current < $count; $this->current++){
			$line = $this->getCurrentLine();
			if(($line != '') && ($line[0] == '|')){
				$elms = explode('|', $line);
				$elmsCount = count($elms);
				if($elmsCount > 2){
					$this->parseTableRow($table, $elms, $elmsCount);
				}
			}else{
				$this->current--;
				return;
			}
		}
	}
	
	function parseTableRow($table, $elms, $elmsCount){
		$last = $elmsCount - 1;
		$row = new CWPukiWikiLikeFormat_TableRow();
		$collection = &$table->body;
		$options = explode(':', trim($elms[$last]));
		$caption = array();
		foreach($options as $option){
			if($option == 'h'){
				$collection = &$table->header;
			}else if($option == 'f'){
				$collection = &$table->footer;
			}else if($option != ''){
				$caption[] = $option;
			}
		}
		if(count($caption) > 0){
			$table->caption = implode(':', $caption);
		}
		$collection[] = $row;
		$x = 0;
		$y = 0;
		for($i = 1; $i < $last; $i++){
			$elm = $elms[$i];
			$countRowItems = count($row->items);
			$x = $countRowItems;
			$y = count($collection) - 1;
			// colspan
			if(($countRowItems > 0) && ($elm == '<')){
				$row->items[$countRowItems - 1]->colspan++;
				$row->items[] = new CWPukiWikiLikeFormat_TableItem($this, '', 0, 0, '', $row->items[$countRowItems - 1]);
			}else{
				$elmlen = strlen($elm);
				if($elmlen > 0){
					// th
					if(($elm[0] == '*') || (($elm[0] == '~') && ($elmlen > 1))){
						$item = new CWPukiWikiLikeFormat_TableItem($this, substr($elm, 1), $x, $y);
						$item->type = 'th';
						$row->items[] = $item;
					// rowspan
					}else if(($elm == '~') && ($collection == $table->body)){
						$countBody = count($table->body);
						if($countBody > 1){
							$super = $table->body[$countBody - 2]->items[$x];
							if($super->isEmpty()){
								$super = $super->spanItem;
							}
							$super->rowspan = $y - $super->y + 1;
							$row->items[] = new CWPukiWikiLikeFormat_TableItem($this, '', 0, 0, '', $super);
						}else{
							$row->items[] = new CWPukiWikiLikeFormat_TableItem($this, '', $x, $y);
						}
					}else{
						$item = new CWPukiWikiLikeFormat_TableItem($this, $elm, $x, $y);
						$row->items[] = $item;
					}
				}else{
					$item = new CWPukiWikiLikeFormat_TableItem($this, $elm, $x, $y);
					$row->items[] = $item;
				}
			}
		}
	}

	function extractBlockPlugin($line){
		$n = preg_match(
			'/^#([a-z0-9_]+)(\(.*?\)|)$/',
			$line,
			$matches);
		if($n > 0){
			switch($matches[1]){
				case 'aa':{
					$block = new CWPukiWikiLikeFormat_Element('pre');
					$block->setAttribute('class', 'AA');
					$this->parseBlockBody($block, '#endaa', true);
					
					$this->document->addChild($block);
					break;
				}
				case 'code':{
					$syntax = substr($matches[2], 1, strlen($matches[2]) - 2);
					$block = new CWPukiWikiLikeFormat_Element('pre');
					if($syntax != ''){
						$block->setAttribute('class', 'brush: ' . $syntax);
					}
					$this->parseBlockBody($block, '#endcode', true);

					$this->document->addChild($block);
					break;
				}
				case 'quote':{
					$block = new CWPukiWikiLikeFormat_Element('blockquote');
					$cite = substr($matches[2], 1, strlen($matches[2]) - 2);
					$this->parseBlockquote($block);
					if($cite != ''){
						$block->addChild('<cite>' . $this->extractInline($cite) . '</cite>');
					}
						
					$this->document->addChild($block);
					break;
				}
				case 'html':{
					$this->parseHtmlBlock();
					break;
				}
				case 'content':{
					$this->document->addChild($this->document->headerList);
					break;
				}
				case 'ls':{
					if(is_page()){
						global $wp_query;
						$postID = $wp_query->post->ID;
						
						//parse_str($matches[2], $args);
						$args = array();
						$args['child_of'] = $postID;
						$args['sort_column'] = 'post_title';
						$args['title_li'] = '';
						$args = $this->parseArguments($args, substr($matches[2], 1, strlen($matches[2]) - 2));
						$args['echo'] = 0;
						
						$query = array();
						foreach(array_keys($args) as $key){
							$query[] = $key . '=' . $args[$key];
						}
						
						$pages = wp_list_pages(implode('&', $query));
						
						$this->document->addChild('<ul class="page-list">' . $pages . '</ul>');
					}
					break;
				}
				case 'require':{
					$path = substr($matches[2], 1, strlen($matches[2]) - 2);
					$page = get_page_by_path($path);
					$parser = new CWPukiWikiLikeFormat_Parser();
					
					$this->document->addChild($parser->parse($page->post_content));
					break;
				}
				case 'clear':{
					$clear = ($matches[2] != '') ? $matches[2] : 'all';
					$this->document->addChild('<br clear="' . $clear . '" />');
					break;
				}
				case 'hr':{
					$this->document->addChild('<hr />');
					break;
				}
			}
		}else{
			$this->document->addChild($this->extractInline($line));
		}
	}

	function parseBlockBody($block, $endword, $escape = false){
		$count = count($this->lines);
		for($this->current++; $this->current < $count; $this->current++){
			$line = $this->getCurrentLine();
			if(strcmp($line, $endword) == 0){
				break;
			}else{
				if(espace){
					$block->addChild(htmlentities($line, ENT_QUOTES, mb_internal_encoding()) . "\r\n");
				}else{
					$block->addChild($this->extractInline($line) . "\r\n");
				}
			}
		}
	}

	function parseBlockquote($block){
		$count = count($this->lines);
		for($this->current++; $this->current < $count; $this->current++){
			$line = $this->getCurrentLine();
			if(strcmp($line, '#endquote') == 0){
				return;
			}else{
				$block->addChild($this->extractInline($line) . "\r\n");
			}
		}
	}

	function parseHtmlBlock(){
		$count = count($this->lines);
		for($this->current++; $this->current < $count; $this->current++){
			$line = $this->getCurrentLine();
			if(strcmp($line, '#endhtml') == 0){
				return;
			}else{
				$this->document->addChild($line . "\r\n");
			}
		}
	}


	function extractInline($content){
		return $this->parseNotes(
		       $this->inlinePlugin(
		       $this->makeStrike(
		       $this->makeIns(
		       $this->makeEm(
		       $this->makeStrong(
		       $this->makeLink(htmlentities($content, ENT_QUOTES, mb_internal_encoding()))))))));
	}

	function inlinePlugin($content){
		return preg_replace_callback(
			'/(&amp;|)&amp;([a-z0-9_]+?)(\((.*?)\)|){(.*?)};/',
			array(&$this, 'inlinePluginCallback'),
			$content);
	}
	
	function inlinePluginCallback($matches){
		$escape = $matches[1];
		$name = $matches[2];
		$arg = $matches[4];
		$body = $matches[5];
		if($escape != ''){
			return substr($matches[0], 5, strlen($matches[0]) - 5);
		}else{
			switch($name){
				case 'cite':
					return '<cite>' . $body . '</cite>';
				case 'ruby':
					return '<ruby><rb>' . $body . '</rb><rp>(</rp><rt>' . $arg . '</rt><rp>)</rp></ruby>';
				case 'emruby':
					return '<ruby><rb>' . $body . '</rb><rt>' . str_repeat('・', mb_strlen($body)) . '</rt></ruby>';
				case 'abbr':
					return '<abbr title="' . $arg . '">' . $body . '</abbr>';
				case 'acronym':
					return '<acronym title="' . $arg . '">' . $body . '</acronym>';
				case 'bdo':
					return '<bdo dir="' . $arg . '">' . $body . '</bdo>';
				case 'sub':
					return '<sub>' . $body . '</sub>';
				case 'sup':
					return '<sup>' . $body . '</sup>';
				case 'kbd':
					return '<kbd>' . $body . '</kbd>';
				case 'sample':
					return '<samp>' . $body . '</samp>';
				case 'var':
					return '<var>' . $body . '</var>';
				case 'q':
					return '<q>' . $body . '</q>';
				case 'code':
					return '<code>' . $body . '</code>';
				case 'rep':
					return '<del>' . $arg . '</del><ins>' . $body . '</ins>';
				case 'aname':
					return '<a id=">' . $body . '"></a>';
				default:
					return $matches[0];
			}
		}
	}

	function parseNotes($content){
		return preg_replace_callback(
			'/\(\((.+?)\)\)/',
			array(&$this, 'parseNotesCallback'),
			$content);
	}
	
	function parseNotesCallback($matches){
		$index = count($this->document->notes) + 1;
		
		global $wp_query;
		$postID = $wp_query->post->ID;
		$id = $postID . '_' . $index;
		
		$footId = 'notefoot_' . $id;
		$textId = 'notetext_' . $id;
		
		$note = new CWPukiWikiLikeFormat_Note($this->extractInline($matches[1]), $footId, $textId, $index);
		$this->document->notes[] = $note;
		
		return $note->toHtmlNoteText();
	}
	
	function makeLink($content){
		return preg_replace_callback(
			'/\[\[(.+?)(|&gt;([^>]+?))\]\]/',
			array(&$this, 'makeLinkCallback'),
			$content);
	}
	
	function makeLinkCallback($matches){
		if(strlen($matches[2]) > 0){
			return '<a href=' . $matches[2] . '>' . $matches[1] . '</a>';
		}else{
			return '<a href=' . $matches[1] . '>' . $matches[1] . '</a>';
		}
	}
	
	function makeStrong($content){
		return preg_replace_callback(
			'/\'\'\'(.+?)\'\'\'/',
			array(&$this, 'makeStrongCallback'),
			$content);
	}
	
	function makeStrongCallback($matches){
		return '<strong>' . $matches[1] . '</strong>';
	}

	function makeEm($content){
		return preg_replace_callback(
			'/\'\'(.+?)\'\'/',
			array(&$this, 'makeEmCallback'),
			$content);
	}
	
	function makeEmCallback($matches){
		return '<em>' . $matches[1] . '</em>';
	}
	
	function makeStrike($content){
		return preg_replace_callback(
			'/%%(.+?)%%/',
			array(&$this, 'makeStrikeCallback'),
			$content);
	}
	
	function makeStrikeCallback($matches){
		return '<del>' . $matches[1] . '</del>';
	}
	
	function makeIns($content){
		return preg_replace_callback(
			'/%%%(.+?)%%%/',
			array(&$this, 'makeInsCallback'),
			$content);
	}
	
	function makeInsCallback($matches){
		return '<ins>' . $matches[1] . '</ins>';
	}
}

class CWPukiWikiLikeFormat_Document{
	public $children;
	public $headerList;
	public $notes = array();
	
	function CWPukiWikiLikeFormat_Document(){
		$this->headerList = new CWPukiWikiLikeFormat_HeaderList();
	}
	
	function addChild($element){
		/*if(!is_string($element) && !is_callable(array($element, 'toHtml'))){
			debug_print_backtrace();
			var_dump(gettype($element));
			unko();
		}*/
		if(is_null($this->children)){
			$this->children = array($element);
		}else{
			$this->children[] = $element;
		}
	}
	
	function toHtml(){
		foreach($this->children as $child){
			if(is_string($child)){
				$html .= $child;
			}else{
				$html .= $child->toHtml();
			}
		}
		
		// note
		if(count($this->notes) > 0){
			$html .= '<dl class="notefoot">';
			foreach($this->notes as $note){
				$html .= $note->toHtmlNoteFoot();
			}
			$html .= '</dl>';
		}
		return $html;
	}
}


class CWPukiWikiLikeFormat_Element{
	public $tag;
	public $children;
	public $attributes;
	
	function CWPukiWikiLikeFormat_Element($tag){
		$this->tag = $tag;
	}
	
	function addChild($element){
		if(is_null($this->children)){
			$this->children = array($element);
		}else{
			$this->children[] = $element;
		}
	}
	
	function setAttribute($key, $value){
		if(is_null($this->attributes)){
			$this->attributes = array($key => $value);
		}else{
			$this->attributes[$key] = $value;
		}
	}
	
	function toHtml(){
		$html = '<' . $this->tag;
		if(!is_null($this->attributes) && count($this->attributes) > 0){
			foreach(array_keys($this->attributes) as $key){
				$html .= ' ' . $key . '="' . $value . '"';
			}
		}
		$html .= '>';
		if(!is_null($this->children)){
			foreach($this->children as $child){
				if(is_string($child)){
					$html .= $child;
				}else{
					$html .= $child->toHtml();
				}
			}
		}
		$html .= '</' . $this->tag . '>';
		return $html;
	}
	
	
}

class CWPukiWikiLikeFormat_ListElement extends CWPukiWikiLikeFormat_Element{
	//private $tag;
	
	function CWPukiWikiLikeFormat_ListElement($ordered = false){
		$this->tag = ($ordered) ? 'ol' : 'ul';
	}
	
	function toHtml(){
		$html = '<' . $this->tag;
		if(!is_null($this->attributes) && count($this->attributes) > 0){
			foreach(array_keys($this->attributes) as $key){
				$html .= ' ' . $key . '="' . $value . '"';
			}
		}
		$html .= '>';
		if(!is_null($this->children)){
			foreach($this->children as $child){
				if(is_string($child)){
					$html .= $child;
				}else{
					$html .= '<li>' . $child->toHtml() . '</li>';
				}
			}
		}
		$html .= '</' . $this->tag . '>';
		return $html;
	}
}

class CWPukiWikiLikeFormat_Table{
	public $header = array();
	public $footer = array();
	public $body = array();
	public $caption = '';
	public $colgroups = array();
	
	function CWPukiWikiLikeFormat_Table(){
	}
	
	function toHtml(){
		$html = "<table>\n";
		if($this->caption != ''){
			$html .= '<caption>' . $this->caption . "</caption>\n";
		}
		if(count($this->colgroups)){
			foreach($this->colgroups as $colgroup){
				$html .= $colgroup->toHtml . "\n";
			}
		}
		if(count($this->header)){
			foreach($this->header as $row){
				$html .= '<thead>' . $row->toHtml(). "</thead>\n";
			}
		}
		if(count($this->footer)){
			foreach($this->footer as $row){
				$html .= '<tfoot>' . $row->toHtml(). "</tfoot>\n";
			}
		}
		if(count($this->body) > 0){
			$html .= "<tbody>\n";
			foreach($this->body as $row){
				$html .= $row->toHtml();
			}
			$html .= "</tbody>\n";
		}
		$html .= "</table>\n";
		return $html;
	}
}

class CWPukiWikiLikeFormat_TableRow{
	public $items = array();
	
	function CWPukiWikiLikeFormat_TableRow(){
	}
	
	function toHtml(){
		$html = '';
		if(count($this->items)){
			$html .= '<tr>';
			foreach($this->items as $item){
				if(!($item->isEmpty())){
					$html .= $item->toHtml();
				}
			}
			$html .= '</tr>';
		}
		return $html . "\n";
	}
}

class CWPukiWikiLikeFormat_TableItem{
	public $content;
	public $type;
	public $spanItem;
	public $colspan = 1;
	public $rowspan = 1;
	public $x;
	public $y;
	public $align = '';
	
	function CWPukiWikiLikeFormat_TableItem($parser, $content = '', $x = 0, $y = 0, $type = 'td', $spanItem = NULL){
		$cell = explode(':', $content);
		$count = count($cell);
		$last = $count - 1;
		for($i = 0; $i < $count; $i++){
			if($i == $last){
				$this->content .= $cell[$i];
			}else if($cell[$i] == 'CENTER'){
				$this->align = 'center';
			}else if($cell[$i] == 'LEFT'){
				$this->align = 'left';
			}else if($cell[$i] == 'RIGHT'){
				$this->align = 'right';
			}else{
				$this->content .= $cell[$i];
			}
		}
		$this->x = $x;
		$this->y = $y;
		$this->type = $type;
		$this->spanItem = $spanItem;
		$this->content = $parser->extractInline($this->content);
	}
	
	function isEmpty(){
		return ($this->spanItem != NULL);
	}
	
	function toHtml(){
		$html = '<' . $this->type;
		if($this->colspan > 1){
			$html .= ' colspan=' . $this->colspan;
		}
		if($this->rowspan > 1){
			$html .= ' rowspan=' . $this->rowspan;
		}
		if($this->align != ''){
			$html .= ' align="' . $this->align . '"';
		}
		$html .= '>' . $this->content . '</' . $this->type . '>';
		return $html;
	}
}

class CWPukiWikiLikeFormat_TableColumnGroup{
	public $span;
	public $columns = array();
	
	function CWPukiWikiLikeFormat_TableColumnGroup($span = 1){
		$this->span = $span;
	}
	
	function toHtml(){
		$html = '<colgroup span=' . $this->span . ">\n";
		foreach($this->columns as $column){
			$html .= $column->toHtml();
		}
		$html .= "</colgroup>\n";
		return $html;
	}
}

class CWPukiWikiLikeFormat_TableColumn{
	public $align = '';
	public $span;
	public $width;
	
	function CWPukiWikiLikeFormat_TableColumn($span = 1){
		$this->span = $span;
	}
	
	function toHtml(){
		$html = '<col';
		$html .= " />\n";
		return $html;
	}
}

class CWPukiWikiLikeFormat_HeaderList{
	public $items = array();
	
	function CWPukiWikiLikeFormat_HeaderList(){
	}
	
	function toHtml(){
		$html = '';
		if(count($this->items) > 0){
			$html = '<ul class="header-list"><h3>' . CWPukiWikiLikeFormat_HeaderTitle . '</h3>';
			$level = NULL;
			foreach($this->items as $item){
				if($level == NULL){
					$level = $item->level;
					$html .= '<li><a href="' . $item->href . '">' . $item->content . '</a>';
				}else if($level < $item->level){
					$level = $item->level;
					$html .= '<ul><li><a href="' . $item->href . '">' . $item->content . '</a>';
				}else if($level > $item->level){
					for($i = $item->level; $i < $level; $i++){
						$html .= '</li></ul>';
					}
					$level = $item->level;
					$html .= '<li><a href="' . $item->href . '">' . $item->content . '</a>';
				}else{
					$html .= '</li><li><a href="' . $item->href . '">' . $item->content . '</a>';
				}
			}
			for($i = 2; $i < $level; $i++){
				$html .= '</li></ul>';
			}
			$html .= "\n";
		}
		return $html;
	}
}

class CWPukiWikiLikeFormat_HeaderListItem{
	public $content;
	public $href;
	public $level;
	
	function CWPukiWikiLikeFormat_HeaderListItem($content, $href, $level = 1){
		$this->content = $content;
		$this->href = $href;
		$this->level = $level;
	}
}

class CWPukiWikiLikeFormat_Note{
	public $content;
	public $footId;
	public $textId;
	public $index;
	
	function CWPukiWikiLikeFormat_Note($content, $footId, $textId, $index){
		$this->content = $content;
		$this->footId = $footId;
		$this->textId = $textId;
		$this->index = $index;
	}
	
	function toHtmlNoteText(){
		return '<a id="' . $this->textId . '" class="note-anchor" href="#' . $this->footId . '" title="' . strip_tags($this->content) . '">*' . $this->index . '</a>';
	}
	
	function toHtmlNoteFoot(){
		return '<dt><a id="' . $this->footId . '" class="note-anchor" href="#' . $this->textId . '">*' . $this->index . '</a></dt><dd>' . $this->content . '</dd>';
	}
}

add_action('init', array('CWPukiWikiLikeFormat', 'init'));
?>