#!/bin/bash

# Script to copy the headers to all the source files and header files
# Usage: ./add-headers.sh [WINDOWS_MACHINE] [FOLDERS_TO_IGNORE]

# Will call dos2unix to remove windows-specific characters from file
CONVERT_FILES_TO_UNIX=${1:-false}

# Folders for globstar to ignore
FOLDERS_TO_IGNORE=${2:-"**/node_modules/**:**/bin/**:**/obj/**:**/build/static/**"}

shopt -s globstar nullglob extglob

# Generated folders to ignore
GLOBIGNORE=$FOLDERS_TO_IGNORE

# Adds header file with single-line comment format, for example:
#   // Copyright (c) Microsoft Corporation. All rights reserved.
#   // Licensed under the MIT License.
function add_header_for_files_singleline_comment()
{
    comment_style=$1
    file_types=$2

    echo "Comment Type: $comment_style"
    echo "File Types: $file_types"
    printf "$$ Copyright (c) Microsoft Corporation. All rights reserved.\\n$$ Licensed under the MIT License.\\n\\n" > /tmp/"$USER-"license
    sed -i "s%$$%$1%g" /tmp/"$USER"-license

    add_header "$file_types"
}

# Adds header file with multi-line comment format, for example:
#   <!-- Copyright (c) Microsoft Corporation. All rights reserved.
#        Licensed under the MIT License. -->
function add_header_for_files_multiline_comment()
{
    comment_style_start=$1
    comment_style_end=$2
    file_types=$3

    begin_comment_flag="$$begin"
    end_comment_flag="$$end"
    spaces_flag="$$spaces"

    # creates string of spaces with the same length as the comment start, for correct spacing for second line of header
    space_string=$(eval printf '" %0.s"' "{1..${#comment_style_start}}")

    echo "Comment Type: $1 <header> $2"
    echo "File Types: $3"
    printf "$begin_comment_flag Copyright (c) Microsoft Corporation. All rights reserved.\\n$spaces_flag Licensed under the MIT License. $end_comment_flag\\n\\n" > /tmp/$USER-license

    sed -i "s#$begin_comment_flag#$comment_style_start#g" /tmp/"$USER"-license
    sed -i "s#$end_comment_flag#$comment_style_end#g" /tmp/"$USER"-license
    sed -i "s#$spaces_flag#$space_string#g" /tmp/"$USER"-license

    add_header "$file_types"
}

# Adds header stored in tmp file to the specified file types
# can look for multiple file types using '|' in parameter, for example "c|cpp|h"
function add_header()
{
    file_types=$1

    for f in **/*.@($file_types); do

    if (grep "Copyright (c) Microsoft Corporation" "$f"); then 
        echo "No need to copy the License Header to $f"
    else
        # needed for windows to convert remove UTF-8 with BOM characters
        if [ "$CONVERT_FILES_TO_UNIX" = true ]; then
            dos2unix "$f"
        fi
        cat "/tmp/$USER-license" "$f" > "$f".new
        mv "$f".new "$f"
        echo "License Header copied to $f"
    fi
    done
}

add_header_for_files_singleline_comment "#" "yaml|yml"
add_header_for_files_multiline_comment "/*" "*/" "css"
add_header_for_files_multiline_comment "<!--" "-->" "html"
add_header_for_files_singleline_comment "//" "cs|tsx"