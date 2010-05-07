*コマンドライン

**findコマンド - ファイル検索

 Nekome.exe find [オプション]

/d or /directory:[ディレクトリパス]
/m or /mask:[マスク]
/r            - 再帰的に検索
/D or /Dialog - ダイアログを表示

***例

 Nekome.exe find /d:c:\ /m:*.exe /r

**grepコマンド - Grep

 Nekome.exe grep [オプション]

/d or /directory:[ディレクトリパス]
/m or /mask:[マスク]
/r or /recursive  - 再帰的に検索
/w or /word:[検索語句]
/e or /Regex      - 正規表現
/i or /ignorecase - 大文字小文字を区別しない
/D or /Dialog     - ダイアログを表示