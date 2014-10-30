qt translation file synchronize tool

usage:
tssync unfinished.ts reference.ts

This operation will enumerate unfinished.ts, found message node in reference.ts.
If reference node found, translation text will copy to unifinished and mark as finished.
If reference node not found, and message node's type attribute equal "unfinished", then it will set translation to source text, and keep unfinished attribute.

warning:
backup unfinished.ts before you use!