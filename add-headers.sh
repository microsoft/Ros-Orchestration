#!/bin/bash

# script to copy the headers to all the source files and header files

shopt -s globstar nullglob extglob

GLOBIGNORE="**/node_modules/**:**/bin/**:**/obj/**:**/build/static/**"

function add_header_for_files_singleline_comment()
{
    echo "Comment Type: $1"
    echo "File Types: $2"
    printf "$$ Copyright (c) Microsoft Corporation. All rights reserved.\\n$$ Licensed under the MIT License.\\n\\n" > /tmp/$USER-license
    sed -i "s%$$%$1%g" /tmp/$USER-license

    add_header $2
}

function add_header_for_files_multiline_comment()
{
    beginComment="$$Begin"
    endComment="$$End"

    echo "Comment Type: $1 <header> $2"
    echo "File Types: $3"
    printf "$beginComment Copyright (c) Microsoft Corporation. All rights reserved.\\nLicensed under the MIT License. $endComment\\n\\n" > /tmp/$USER-license

    sed -i "s#$beginComment#$1#g" /tmp/$USER-license
    sed -i "s#$endComment#$2#g" /tmp/$USER-license

    add_header $3
}

function add_header()
{
    for f in **/*.@($1); do
    if (grep "Copyright (c) Microsoft Corporation" "$f"); then 
        echo "No need to copy the License Header to $f"
    else
        cat "/tmp/$USER-license" "$f" > "$f".new
        mv "$f".new "$f"
        echo "License Header copied to $f"
    fi 
    done
}

add_header_for_files_singleline_comment "#" "cs|yaml|yml"
add_header_for_files_multiline_comment "/*" "*/" "css"
add_header_for_files_multiline_comment "<!--" "-->" "html"
add_header_for_files_singleline_comment "//" "tsx"