import re

from PyQt5.QtGui import QSyntaxHighlighter
class Highlighter(QSyntaxHighlighter):
    def __init__(self, parent=None):
        super().__init__(parent)
        self.mapping = {}

    def add_mapping(self, pattern, pattern_format):
        self.mapping[pattern] = pattern_format

    def highlightBlock(self, text_block: str):
        for pattern, fmt in self.mapping.items():
            for match in re.finditer(pattern, text_block):
                start, end = match.span()
                self.setFormat(start, end-start, fmt)
