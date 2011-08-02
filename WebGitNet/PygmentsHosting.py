import clr
pygments = clr.AddReference('Pygments')

from pygments import highlight
from pygments.lexers import guess_lexer, get_lexer_for_filename
from pygments.formatters import HtmlFormatter

def hilight_file(filename, contents):
    try:
      lexer = get_lexer_for_filename(filename)
    except:
      lexer = get_lexer_for_filename('fallback.txt')
    formatter = HtmlFormatter(linenos=True, cssclass="code")
    return highlight(contents, lexer, formatter)

def hilight_text(text):
    try:
      lexer = guess_lexer(text)
    except:
      lexer = get_lexer_for_filename('fallback.txt')
    formatter = HtmlFormatter(linenos=True, cssclass="code")
    return highlight(text, lexer, formatter)
