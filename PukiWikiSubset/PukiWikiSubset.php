<?php
/*
Plugin Name: PukiWikiSubset
Plugin URI: http://nekoaruki.com/wp/downloads/wordpressplugins/pukiwikisubset
Description: PukiWiki記法の一部が使用できるようになります。
Author: cat-walk
Version: 0.8
Author URI: http://nekoaruki.com

ver0.8
-引用記法変更
-pre記法廃止
-code記法追加
-aa記法追加
-sub記法追加
-sup記法追加
-kbd記法追加
-sample記法追加
-var記法追加

ver0.7
-cite記法を追加。

ver0.6
-emruby記法を追加。

ver0.5
-&date;に対応。
-&time;に対応。
-&now;に対応。
-&t;に対応。
-コメント行に対応。
-注釈に対応。
-ヘッダーのアンカー名の付け方を変更。
-ヘッダー周りのバグを修正。

ver0.4
-#lsに対応。
-#clearに対応。

ver0.3
-表組みでPukiWikiでは">"で右側のセルに統合するところを、左側のセルに統合していたのを修正。
-表組みで"<"という内容のセルは左側のセルに統合するようにした。
-表組みでキャプションを指定出来るようにした。
-表組みでthの指定をセルの先頭に"*"でも可能にした(そっちの方が直感的だから)。
-ルビ記法に対応。
-XHTMLではstrikeが非推奨だったので、delにした。
-ins記法を追加。
-abbr記法を追加。
-acronym記法を追加。
-bdo記法を追加。
-見出しリストに対応。

ver0.2
表組み記法に対応。

ver0.1
初版

ToDo:
colgroupに対応する。
*/

if(class_exists('CWPukiWikiSubset')){
	$_cwPukiWikiSubset = new CWPukiWikiSubset();
}

class CWPukiWikiSubset{
	public static $headerTitle = '見出し';
	public static $anchorChar = '&dagger;';
	public $headerList;
	public $notes;

	function CWPukiWikiSubset(){
		$this->headerList =  new CWPukiWikiSubset_HeaderList();
		// Hooks
		add_filter('the_excerpt', array(&$this, 'scan'), 3);
		add_filter('the_content', array(&$this, 'scan'), 3);
		add_filter('content_save_pre', array(&$this, 'preprocess'));
	}
	
	function preprocess($content){
		$lines = explode("\r\n", $content);
		$returnContent = array();
		foreach($lines as $line){
			$returnContent[] = preg_replace_callback(
				'/(?!&)&([a-z0-9_]+?);/',
				array('CWPukiWikiSubset', 'preprocessCallback'),
				$line);
		}
		return implode("\r\n", $returnContent);
	}
	
	function preprocessCallback($matches){
		switch($matches[1]){
			case 'date':
				return date(get_option('date_format'));
			case 'time':
				return date(get_option('time_format'));
			case 'now':
			case 'datetime':
				return date(get_option('date_format')) . ' ' . date(get_option('time_format'));
			case 't':
				return "\t";
			default:
				return $matches[0];
		}
	}

	
	function scan($content){
		$this->headerList->items = array();
		$contentList = array();
		$this->notes = array();
		$lines = explode("\r\n", $content);
		
		$count = count($lines);
		for($current = 0; $current < $count; $current++){
			$line = $lines[$current];
			if($line != ''){
				// リスト
				$offset = $line[0];
				switch($offset){
					case '-':{
						$list = new CWPukiWikiSubset_ListContainer('ul', 1);
						$list->items[] = new CWPukiWikiSubset_ListItem(substr($line, 1), 1);
						$this->parseUOList($lines, $current, $offset, $list, 1);
						
						$contentList[] = $list;
						break;
					}
					case '+':{
						$list = new CWPukiWikiSubset_ListContainer('ol', 1);
						$list->items[] = new CWPukiWikiSubset_ListItem(substr($line, 1), 1);
						$this->parseUOList($lines, $current, $offset, $list, 1);
						
						$contentList[] = $list;
						break;
					}
					// 定義リスト
					case ':':{
						$pos = strpos($line, '|');
						if($pos > 0){
							$list = new CWPukiWikiSubset_ListContainer('dl');
							$list->items[] = new CWPukiWikiSubset_DefinitionListItem(substr($line, 1, $pos - 1), substr($line, $pos + 1));
							$this->parseDList($lines, $current, $list);
							
							$contentList[] = $list;
						}else{
							$contentList[] = new CWPukiWikiSubset_TextItem($this->extractInline($line));
						}
						break;
					}
					// 見出し
					case '*':{
						$contentList[] = new CWPukiWikiSubset_TextItem(preg_replace_callback(
						'/^(\*+)(|\[[^\]]+\])(.+)$/m',
						array(&$this, 'makeHeader'),
						$line));
						break;
					}
					// 表組み
					case '|':{
						$elms = explode('|', $lines[$current]);
						$elmsCount = count($elms);
						if($elmsCount > 2){
							$table = new CWPukiWikiSubset_Table();
							$this->addTableRow($table, $elms, $elmsCount);
							$this->parseTable($lines, $current, $table);
							$contentList[] = $table;
						}
						break;
					}
					// ブロックプラグイン
					case '#':{
						$this->extractBlockPlugin($lines, $current, $contentList);
						break;
					}
					// コメント行
					default:{
						if(($offset == '/') && (strpos($line, '//') === 0)){
							continue;
						}else{
							$contentList[] = new CWPukiWikiSubset_TextItem($this->extractInline($line));
						}
					}
				}
			}else{
				$contentList[] = new CWPukiWikiSubset_TextItem('');
			}
		}
		$returnContent = '';
		foreach($contentList as $item){
			$returnContent .= $item->toHTML() . "\n";
		}
		
		if(count($this->notes) > 0){
			$returnContent .= '<dl class="notefoot">';
			foreach($this->notes as $note){
				$returnContent .= $note->toHTMLNoteFoot();
			}
			$returnContent .= '</dl>';
		}
		
		return $returnContent;
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
	
	function extractBlockPlugin($lines, &$current, &$contentList){
		global $_cwPukiWikiSubset;
		$n = preg_match(
			'/^#([a-z0-9_]+)(\(.*?\)|)$/',
			$lines[$current],
			$matches);
		if($n > 0){
			switch($matches[1]){
				case 'aa':{
					$block = new CWPukiWikiSubset_PreElement();
					$block->attr = 'class="AA"';
					$this->parseBlockBody($lines, $current, $block, '#endaa');
					
					$contentList[] = $block;
					break;
				}
				case 'code':{
					$args = array();
					$syntax = substr($matches[2], 1, strlen($matches[2]) - 2);
					$block = new CWPukiWikiSubset_PreElement();
					if($syntax != ''){
						$block->attr = 'class="brush: ' . $syntax . '"';
					}
					$this->parseBlockBody($lines, $current, $block, '#endcode');

					$contentList[] = $block;
					break;
				}
				case 'quote':{
					$list = new CWPukiWikiSubset_ListContainer('blockquote');
					$cite = substr($matches[2], 1, strlen($matches[2]) - 2);
					$this->parseBlockquote($lines, $current, $list);
					if($cite != ''){
						$list->items[] = new CWPukiWikiSubset_TextItem('<cite>' . $this->extractInline($cite) . '</cite>');
					}
						
					$contentList[] = $list;
					break;
				}
				case 'content':{
					$contentList[] = $this->headerList;
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
						
						$contentList[] = new CWPukiWikiSubset_TextItem('<ul class="page-list">' . str_replace("\n", '', $pages) . '</ul>');
					}
					break;
				}
				case 'clear':{
					$clear = ($matches[2] != '') ? $matches[2] : 'all';
					$contentList[] = new CWPukiWikiSubset_TextItem('<br clear="' . $clear . '" />');
					break;
				}
				case 'hr':{
					$contentList[] = new CWPukiWikiSubset_TextItem('<hr />');
					break;
				}
			}
		}else{
			$contentList[] = new CWPukiWikiSubset_TextItem($_cwPukiWikiSubset->extractInline($lines[$current]));
		}
	}
	
	function parseUOList($lines, &$current, $offset, $list, $level = 1){
		$current++;
		$count = count($lines);
		$offsetlen = strlen($offset);
		for(; $current < $count; $current++){
			if(strpos($lines[$current], $offset) === 0){
				if(strlen($lines[$current]) > $offsetlen){
					$type = '';
					$c = $lines[$current][$offsetlen];
					if($c == '-'){
						$type = 'ul';
					}else if($c == '+'){
						$type = 'ol';
					}
					if($type != ''){
						$level2 = $level + 1;
						$offset2 = $offset . $lines[$current][$offsetlen];
						$list2 = new CWPukiWikiSubset_ListContainer($type, $level2);
						$list->items[count($list->items) - 1]->containers[] = $list2;
						$list2->items[] = new CWPukiWikiSubset_ListItem(substr($lines[$current], $offsetlen + 1), $level2);
						$this->parseUOList($lines, $current, $offset2, $list2, $level2);
						continue;
					}else{
						$list->items[] = new CWPukiWikiSubset_ListItem(substr($lines[$current], $offsetlen), $level);
					}
				}else{
					$list->items[] = new CWPukiWikiSubset_ListItem('', $level);
				}
			}else{
				$current--;
				return;
			}
		}
	}
	
	function parseDList($lines, &$current, $list){
		$current++;
		$count = count($lines);
		for(; $current < $count; $current++){
			$line = $lines[$current];
			if(($line != '') && ($line[0] == ':')){
				$pos = strpos($line, '|');
				if($pos > 0){
					$list->items[] = new CWPukiWikiSubset_DefinitionListItem(substr($line, 1, $pos - 1), substr($line, $pos + 1), 1);
				}else{
					$current--;
					return;
				}
			}else{
				$current--;
				return;
			}
		}
	}
	
	function parseBlockBody($lines, &$current, $block, $endword){
		$current++;
		$count = count($lines);
		for(; $current < $count; $current++){
			if(strcmp($lines[$current], $endword) == 0){
				break;
			}else{
				$block->text .= $lines[$current] . "\r\n";
			}
		}
		$block->text = htmlentities($block->text, ENT_QUOTES, mb_internal_encoding());
	}
	
	function parseBlockquote($lines, &$current, $list){
		$current++;
		$count = count($lines);
		for(; $current < $count; $current++){
			if(strcmp($lines[$current], '#endquote') == 0){
				return;
			}else{
				$list->items[] = new CWPukiWikiSubset_TextItem($this->extractInline($lines[$current]));
			}
		}
	}
	
	function parseTable($lines, &$current, $table){
		$current++;
		$count = count($lines);
		for(; $current < $count; $current++){
			if(($lines[$current] != '') && ($lines[$current][0] == '|')){
				$elms = explode('|', $lines[$current]);
				$elmsCount = count($elms);
				if($elmsCount > 2){
					$this->addTableRow($table, $elms, $elmsCount);
				}
			}else{
				$current--;
				return;
			}
		}
	}
	
	function addTableRow($table, $elms, $elmsCount){
		$last = $elmsCount - 1;
		$row = new CWPukiWikiSubset_TableRow();
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
		$colspan = 1;
		for($i = 1; $i < $last; $i++){
			$countRowItems = count($row->items);
			$x = $countRowItems;
			$y = count($collection) - 1;
			// colspan
			if(($countRowItems > 0) && ($elms[$i] == '<')){
				$row->items[$countRowItems - 1]->colspan++;
				$row->items[] = new CWPukiWikiSubset_TableItem('', 0, 0, '', $row->items[$countRowItems - 1]);
			// colspan
			}else if(($elms[$i] == '>') && ($i != ($last - 1))){
				$colspan++;
			}else{
				$elmslen = strlen($elms[$i]);
				if($elmslen > 0){
					// th
					if(($elms[$i][0] == '*') || (($elms[$i][0] == '~') && ($elmslen > 1))){
						$item = $this->buildItem(substr($elms[$i], 1), $x, $y);
						$item->colspan = $colspan;
						$colspan = 1;
						$item->type = 'th';
						$row->items[] = $item;
					// rowspan
					}else if(($elms[$i] == '~') && ($collection == $table->body)){
						$countBody = count($table->body);
						if($countBody > 1){
							$super = $table->body[$countBody - 2]->items[$x];
							if($super->isEmpty()){
								$super = $super->spanItem;
							}
							$super->rowspan = $y - $super->y + 1;
							$row->items[] = new CWPukiWikiSubset_TableItem('', 0, 0, '', $super);
						}else{
							$row->items[] = new CWPukiWikiSubset_TableItem('', $x, $y);
						}
					}else{
						$item = $this->buildItem($elms[$i], $x, $y);
						$item->colspan = $colspan;
						$colspan = 1;
						$row->items[] = $item;
					}
				}else{
					$item = $this->buildItem($elms[$i], $x, $y);
					$item->colspan = $colspan;
					$colspan = 1;
					$row->items[] = $item;
				}
			}
		}
	}
	
	function buildItem($content, $x, $y){
		$cell = explode(':', $content);
		$item = new CWPukiWikiSubset_TableItem('', $x, $y);
		$count = count($cell);
		$last = $count - 1;
		for($i = 0; $i < $count; $i++){
			if($i == $last){
				$item->content .= $cell[$i];
			}else if($cell[$i] == 'CENTER'){
				$item->align = 'center';
			}else if($cell[$i] == 'LEFT'){
				$item->align = 'left';
			}else if($cell[$i] == 'RIGHT'){
				$item->align = 'right';
			}else{
				$item->content .= $cell[$i];
			}
		}
		return $item;
	}
	
	function makeHeader($matches){
		global $wp_query;
		$postID = $wp_query->post->ID;
		$level = 2 + strlen($matches[1]);
		$anchor = ($matches[2] != '') ?
		          substr($matches[2], 1, strlen($matches[2] - 2)) :
		          $postID . '_' . (count($this->headerList->items) + 1) . '_h' . $level;
		$content = CWPukiWikiSubset::extractInline(rtrim($matches[3]));
		$this->headerList->items[] = new CWPukiWikiSubset_HeaderListItem($content, '#' . $anchor, $level);
		return '<h' . $level . '>' . $content .
		       '<a class="header-anchor" id="' . $anchor . '" href="#'. $anchor . '">' . CWPukiWikiSubset::$anchorChar . '</a></h' . $level . '>';
	}
	
	function parseArguments($a, $args){
		foreach(explode(',', $args) as $elm){
			$pos = strpos($elm, '=');
			$key = '';
			if($pos){
				$key = trim(substr($elm, 0, $pos));
				$value = trim(substr($elm, $pos + 1));
			}else{
				$key = trim($elm);
				$value = 'true';
			}
			$a[$key] = $value;
		}
		return $a;
	}
	
	function extractInline($content){
		return preg_replace('/&amp;&amp;(time|date|datetime|now|t);/', '&amp;$1;',
		       CWPukiWikiSubset::parseNotes(
		       CWPukiWikiSubset::inlinePlugin(
		       CWPukiWikiSubset::makeStrike(
		       CWPukiWikiSubset::makeIns(
		       CWPukiWikiSubset::makeEm(
		       CWPukiWikiSubset::makeStrong(
		       CWPukiWikiSubset::makeLink(htmlentities($content, ENT_QUOTES, mb_internal_encoding())))))))));
	}
	
	function parseNotes($content){
		return preg_replace_callback(
			'/\(\((.+?)\)\)/',
			array(&$this, 'parseNotesCallback'),
			$content);
	}
	
	function parseNotesCallback($matches){
		$note = new CWPukiWikiSubset_Note($this->extractInline($matches[1]));
		$this->notes[] = $note;
		
		$idx = count($this->notes);
		
		global $wp_query;
		$postID = $wp_query->post->ID;
		$id = $postID . '_' . $idx;
		
		$note->footId = 'notefoot_' . $id;
		$note->textId = 'notetext_' . $id;
		$note->index = $idx;
		
		return $note->toHTMLNoteText();
	}
	
	function makeLink($content){
		return preg_replace_callback(
			'/\[\[(.+?)\>([^>]+?)\]\]/',
			array(&$this, 'makeLinkCallback'),
			$content);
	}
	
	function makeLinkCallback($matches){
		return '<a href=' . $matches[2] . '>' . $matches[1] . '</a>';
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

class CWPukiWikiSubset_BlockElement{
	public $tag;
	public $attr;
	public $items = array();
	
	function CWPukiWikiSubset_BlockElement($tag){
		$this->tag = $tag;
	}

	function toHTML(){
		$html .= '<' . $this->tag . ' ' . $this->attr . ">\n";
		foreach($this->items as $item){
			$html .= $item->toHTML();
		}
		$html .= '</' . $this->tag . ">\n";
		return $html;
	}
}

class CWPukiWikiSubset_PreElement{
	public $attr;
	public $text = '';
	public $items = array();
	
	function CWPukiWikiSubset_PreElement(){
	}

	function toHTML(){
		$html .= '<pre ' . $this->attr . ">\n";
		$html .= $this->text;
		$html .= "</pre>\n";
		return $html;
	}
}

class CWPukiWikiSubset_ListContainer{
	private $type;
	private $level;
	public $items = array();
	
	function CWPukiWikiSubset_ListContainer($type, $level = 1){
		$this->type = $type;
		$this->level = $level;
	}
	
	function toHTML(){
		//$html .= $indent . '<' . $this->type . ' class="listlevel-' . $this->level . '">' . "\n";
		$html .= '<' . $this->type . ">\n";
		foreach($this->items as $item){
			$html .= $item->toHTML() . "\n";
		}
		$html .= '</' . $this->type . ">\n";
		return $html;
	}
}

class CWPukiWikiSubset_ListItem{
	private $content;
	private $level;
	public $containers = array();
	
	function CWPukiWikiSubset_ListItem($content, $level = 1){
		$this->content = rtrim($content);
		$this->level = $level;
	}
	
	function toHTML(){
		global $_cwPukiWikiSubset;
		$html .= $indent . '<li>' . $_cwPukiWikiSubset->extractInline($this->content);
		foreach($this->containers as $container){
			$html .= $container->toHTML();
		}
		$html .= '</li>';
		return $html;
	}
}

class CWPukiWikiSubset_DefinitionListItem{
	private $title;
	private $definition;
	private $level;
	
	function CWPukiWikiSubset_DefinitionListItem($title, $definition, $level = 1){
		$this->title = rtrim($title);
		$this->definition = rtrim($definition);
		$this->level = $level;
	}
	
	function toHTML(){
		global $_cwPukiWikiSubset;
		return '<dt>' . $_cwPukiWikiSubset->extractInline($this->title) . "</dt>\n<dd>" . $_cwPukiWikiSubset->extractInline($this->definition) . "</dd>\n";
	}
}

class CWPukiWikiSubset_TextItem{
	private $content;
	
	function CWPukiWikiSubset_TextItem($content){
		$this->content = $content;
	}
	
	function toHTML(){
		return $this->content;
	}
}

class CWPukiWikiSubset_Table{
	public $header = array();
	public $footer = array();
	public $body = array();
	public $caption = '';
	public $colgroups = array();
	
	function CWPukiWikiSubset_Table(){
	}
	
	function toHTML(){
		$html = "<table>\n";
		if($this->caption != ''){
			$html .= '<caption>' . $this->caption . "</caption>\n";
		}
		if(count($this->colgroups)){
			foreach($this->colgroups as $colgroup){
				$html .= $colgroup->toHTML . "\n";
			}
		}
		if(count($this->header)){
			foreach($this->header as $row){
				$html .= '<thead>' . $row->toHTML(). "</thead>\n";
			}
		}
		if(count($this->footer)){
			foreach($this->footer as $row){
				$html .= '<tfoot>' . $row->toHTML(). "</tfoot>\n";
			}
		}
		if(count($this->body) > 0){
			$html .= "<tbody>\n";
			foreach($this->body as $row){
				$html .= $row->toHTML();
			}
			$html .= "</tbody>\n";
		}
		$html .= "</table>\n";
		return $html;
	}
}

class CWPukiWikiSubset_TableRow{
	public $items = array();
	
	function CWPukiWikiSubset_TableRow(){
	}
	
	function toHTML(){
		$html = '';
		if(count($this->items)){
			$html .= '<tr>';
			foreach($this->items as $item){
				if(!($item->isEmpty())){
					$html .= $item->toHTML();
				}
			}
			$html .= '</tr>';
		}
		return $html . "\n";
	}
}

class CWPukiWikiSubset_TableItem{
	public $content;
	public $type;
	public $spanItem;
	public $colspan = 1;
	public $rowspan = 1;
	public $x;
	public $y;
	public $align = '';
	
	function CWPukiWikiSubset_TableItem($content = '', $x = 0, $y = 0, $type = 'td', $spanItem = NULL){
		$this->content = $content;
		$this->x = $x;
		$this->y = $y;
		$this->type = $type;
		$this->spanItem = $spanItem;
	}
	
	function isEmpty(){
		return ($this->spanItem != NULL);
	}
	
	function toHTML(){
		global $_cwPukiWikiSubset;
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
		$html .= '>' . $_cwPukiWikiSubset->extractInline($this->content) . '</' . $this->type . '>';
		return $html;
	}
}

class CWPukiWikiSubset_TableColumnGroup{
	public $span;
	public $columns = array();
	
	function CWPukiWikiSubset_TableColumnGroup($span = 1){
		$this->span = $span;
	}
	
	function toHTML(){
		$html = '<colgroup span=' . $this->span . ">\n";
		foreach($this->columns as $column){
			$html .= $column->toHTML();
		}
		$html .= "</colgroup>\n";
		return $html;
	}
}

class CWPukiWikiSubset_TableColumn{
	public $align = '';
	public $span;
	public $width;
	
	function CWPukiWikiSubset_TableColumn($span = 1){
		$this->span = $span;
	}
	
	function toHTML(){
		$html = '<col';
		$html .= " />\n";
		return $html;
	}
}

class CWPukiWikiSubset_HeaderList{
	public $items = array();
	
	function CWPukiWikiSubset_HeaderList(){
	}
	
	function toHTML(){
		$html = '';
		if(count($this->items) > 0){
			$html = '<ul class="header-list"><h3>' . CWPukiWikiSubset::$headerTitle . '</h3>';
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

class CWPukiWikiSubset_HeaderListItem{
	public $content;
	public $href;
	public $level;
	
	function CWPukiWikiSubset_HeaderListItem($content, $href, $level = 1){
		$this->content = $content;
		$this->href = $href;
		$this->level = $level;
	}
}

class CWPukiWikiSubset_Note{
	public $content;
	public $footId;
	public $textId;
	public $index;
	
	function CWPukiWikiSubset_Note($content){
		$this->content = $content;
	}
	
	function toHTMLNoteText(){
		return '<a id="' . $this->textId . '" class="note-anchor" href="#' . $this->footId . '" title="' . strip_tags($this->content) . '">*' . $this->index . '</a>';
	}
	
	function toHTMLNoteFoot(){
		return '<dt><a id="' . $this->footId . '" class="note-anchor" href="#' . $this->textId . '">*' . $this->index . '</a></dt><dd>' . $this->content . '</dd>';
	}
}
?>