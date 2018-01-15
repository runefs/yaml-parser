#time

#load "../.paket/load/netcoreapp2.0/main.group.fsx"
#load "prelude.fs"
#load "types.fs"
#load "primitives.fs"
#load "flow-styles.fs"
#load "block-styles.fs"
#load "parser.fs"

open FParsec

open YamlParser
open YamlParser.Types

Parser.run "{unquoted : 'separate', http://foo.com, 42: , : omitted key,}"

Parser.run @"{
omitted value:,
http://foo.com,
? 42:,
? 43: ,
'empty':
}"

Parser.run @"{
'adjacent':value,
'readable': value,
'empty':
}"

Parser.run @"{
? explicit: entry,
implicit: entry,
?
}"

Parser.run @"{
unquoted : 'separate',
http://foo.com,
omitted value:,
: omitted key,
}"

Parser.run @"- 1
- 2"

Parser.run @"- 1
-   true
- 3"

Parser.run @"   - 1
   - 2"

Parser.run @"- - 'one' # Compact
  - 'two' # sequence"

Parser.run @"? - 1
  - 2
: 3"

Parser.run @"? - 1
  - 2 : 3
: 4"

Parser.run @"- ::vector
- Up, up, and away!
- -123
- http://example.com/foo#bar"

Parser.run @"[
# my comment
""double
 quoted"", 'single
           quoted',
   [ nested ]
]"

Parser.run @"- [ one, two, ]
- [three ,four]"

Parser.run @"[ 1, 2,
3
, 4
,
5]"


Parser.run @"'quoted key':
- entry"
