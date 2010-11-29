*概要
C#+WPFで出来たGrepソフト。.NetFramework 4.0が必要。

-[[Microsoft .NET Framework 4 (Web インストーラー)>http://www.microsoft.com/downloads/details.aspx?FamilyID=9cfb2d51-5ff4-4491-b0e5-b386f32c0992&displaylang=ja]]
-[[SVN Repository>http://code.google.com/p/catwalkagogo/source/browse/trunk#trunk/Nekome]]

*コマンドライン
Nekome.exe (オプション) (ディレクトリ) (オプション)

オプションは末尾に+もしくは-を付けてオン・オフ指定する。また、各オプションは先頭文字に省略可能。(/pattern→/p、/recursive→/rec)

|ディレクトリ       |検索を開始するディレクトリ                                    |
|/pattern:[パターン]|検索語句                                                      |
|/ignorecase(+|-)   |大文字小文字を区別するかどうか                                |
|/recursive(+|-)    |再帰的に検索するかどうか                                      |
|/mask:[マスク]     |ファイルマスクを指定                                          |
|/regex(+|-)        |正規表現を使用するかどうか                                    |
|/immediately(+|-)  |検索ダイアログを出さずにすぐ検索を始める(他のオプションと併用)|

*更新履歴
**ver1.0.1.0
-自動補完の動作を改良。
-タブが無いときにCtrl+Wでプログラムを終了するようにした。
-Escで検索を中止するようにした。
-起動時に更新を確認できるようにした。

**ver1.0.0.0
-公開