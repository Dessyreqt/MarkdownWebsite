# MarkdownWebsite
A small CLI utility for making a website using Markdown and Handlebars.

## Usage
### Creating a website
Using MarkdownWebsite is pretty easy. Once you have the binary created, invoke it as follows: 

`MarkdownWebsite.exe website -i "C:\website\src" -o "C:\website\static" -l "C:\website\layout"`

`-i` is the input directory

`-o` is the output directory

`-l` is the layout directory

As long as your layout is structured properly, this should produce a website from your layouts compiling all markdown files into html and copying over any other file present in your source directory.

Warning: MarkdownWebsite will delete the contents of the output directory prior to performing any compilation or copying activities.

Handlebars directives are only processed on Markdown files.

### Compiling Handlebars templates

MarkdownWebsite can also be used as a simple command-line Handlebars template compiler. Use this command:

`MarkdownWebsite.exe handlebars -i "C:\website\src\somefile.html" -o "C:\website\static\somefile.html" -p "C:\website\layout\partials"`

`-i` is the input file

`-o` is the output file

`-l` is the partials directory

As you can see you can use this to compile templates with any prior extension.

### Compiling Markdown

Finally, MarkdownWebsite can also be used as a simple command-line Markdown compiler. Use this command:

`MarkdownWebsite.exe markdown -i "C:\website\src\somefile.md" -o "C:\website\static\somefile.html"`

`-i` is the input file

`-o` is the output file

## File structure

There are no restrictions on the layout of your input folder (please don't put your layout folder in it!), but the `layout` folder has certain requirements:
- Any assets that you want to refer to globally should go in the `assets` folder, and you can refer to this in your markdown or templates by using the helper `{{asset 'path/to/image.png'}}`. The compiler will automatically figure out the correct relative path to that asset when compiling.
- Any partials should be placed in the `partials` folder of the layout folder.

Please refer to the example for sample input and output.

## Some possible gotchas

There are some quirks that you'll have to keep in mind when composing your website in markdown:
- Try to keep handlebars statements to not use spaces. Due to the way HandlebarsDotNet works `{{#>page}}` works but `{{#> page }}` does not.
- If the first line after the `@partial-block` in one of your templates is indented four spaces, it can get interpretted by the markdown compiler as preformatted code and thus mangled. Try not to do that.

## To be added
- Support for Handlebars helpers in files?
- Support for non-standard Markdown formats (including Github-flavored markdown)
