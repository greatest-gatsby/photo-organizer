PhotographySorter helps you centralize your image collection. It can combine multiple source directories into one or more target directories, optionally even constructing a human-navigable structure. Config files are simple flat files.

Usage typically follows this pattern:
1. Add source directories \
`add source D:\Art dart` \
`add source D:\Series\Photography photgraphy`

2. Add a target directory \
`add target \\my-nas\James nas`

3. Execute a move \
`move dart nas`

Directories can contain unrelated directories, but they will be automatically included unless explicitly blacklisted or the directory is another entry in the targets/sources list.