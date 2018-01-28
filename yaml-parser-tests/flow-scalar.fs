module YamlParser.FlowStyle.Scalar.Tests

open YamlParser.Types
open YamlParser

open Expecto
open Expecto.Flip

open Prelude

let parser = Parser.bareDocument

[<Tests>]
let tests = testList "flow-scalar"
              [ test "parsing plain text" {
                  let parsePlain text expect = 
                    testParser parser text
                    |> succeed (Expect.equal "proper plain scalar" expect)
                    
                  let parsePlainText text = parsePlain text (String text)

                  parsePlainText "::vector"
                  parsePlainText @"Up, up, and away!"
                  parsePlain @"-123" (String "-123")
                  parsePlainText @"http://example.com/foo#bar"
                }

                test "parse folded" {
                  testParser parser "'trimmed\r\n  \r\n \r\n\r\nas\r\nspace'"
                  |> succeed
                      (Expect.equal
                        "flow folded properly parsed"
                        (String "trimmed\n\n\nas space"))

                  testParser parser @"'trimmed



as
space'"
                  |> succeed
                      (Expect.equal
                        "flow folded properly parsed"
                        (String "trimmed\n\n\nas space"))
                  
                  testParser parser "'\r\n  foo \r\n\r\n  \t bar\r\n\r\n  baz\r\n'"
                  |> succeed 
                      (Expect.equal
                        "flow folded properly parsed"
                        (String " foo\nbar\nbaz "))
                }

                test "single quoted with single quote inside" {
                  testParser parser "'here''s to \"quotes\"'"
                  |> succeed
                      (Expect.equal
                        "with single quote inside properly parsed"
                        (String "here's to \"quotes\""))
                }

                test "single quoted multiline" {
                  testParser parser "' 1st non-empty\r\n\r\n 2nd non-empty\r\n\t3rd non-empty '"
                  |> succeed
                      (Expect.equal
                        "multiline properly parsed"
                        (String " 1st non-empty\n2nd non-empty 3rd non-empty "))
                }

                test "double quoted string in implicit mapping" {
                  testParser parser "\"implicit block key\" : [\r\n  \"implicit flow key\" : value,\r\n ]"
                  |> succeed
                      (Expect.equal
                        "implicit mapping"
                        (Mapping
                          [ String "implicit block key",
                            Sequence 
                              [ Mapping
                                  [ String "implicit flow key", String "value" ]
                              ]
                          ]))
                }

                test "double quoted string line break" {
                  testParser parser "\"\r\n  foo \r\n\r\n  \t bar\r\n\r\n  baz\r\n\""
                  |> succeed 
                      (Expect.equal
                        "foo bar baz line break"
                        (String " foo\nbar\nbaz "))

                  testParser parser "\"folded \r\nto a space,\t\r\n \r\nto a line feed, or \t\\\r\n \\ \tnon-content\""
                  |> succeed 
                      (Expect.equal
                        "folded to a space or line feed or non-content"
                        (String "folded to a space,\nto a line feed, or \t \tnon-content"))

                  testParser parser "\" 1st non-empty\r\n\r\n 2nd non-empty \r\n\t 3rd non-empty \""
                  |> succeed
                      (Expect.equal
                        "non-empty 1,2,3"
                        (String " 1st non-empty\n2nd non-empty 3rd non-empty "))
                }
              ]