import sys

from PyQt5.QtCore import Qt
from PyQt5.QtGui import QTextCharFormat, QFont, QFontDatabase
from PyQt5.QtWidgets import QApplication, QMainWindow, QVBoxLayout, QPlainTextEdit
from highlighter import Highlighter


class Main(QMainWindow):
    def __init__(self):
        super(Main, self).__init__()

        self.setWindowTitle("IDE")
        self.resize(500,500)

        self.layout = QVBoxLayout()
        self.highlighter = Highlighter()
        self.setup_editor()
        self.layout.addWidget(self.editor)
        self.setLayout(self.layout)
        self.setCentralWidget(self.editor) # Hinzufügen des Editors zum zentralen Widget des MainWindow

    def setup_editor(self):
        class_format = QTextCharFormat()
        class_format.setForeground(Qt.blue)
        class_format.setFontWeight(QFont.Bold)
        pattern = r'^\s*class\s+\w+\(.*$'
        self.highlighter.add_mapping(pattern, class_format)

        output_format = QTextCharFormat()
        output_format.setForeground(Qt.GlobalColor.darkRed)
        #output_format.setFontWeight(QFont.Weight.Bold)
        pattern = r'^\s*out\s*\(\b.*\b\)$'
        self.highlighter.add_mapping(pattern, output_format)

        self.editor = QPlainTextEdit()

        font = QFontDatabase.systemFont(QFontDatabase.SystemFont.FixedFont)
        self.editor.setFont(font)

        self.highlighter.setDocument(self.editor.document()) #TODO Ist die Zeile da, dann kann man nichts schreiben und Programm stürzt ab
        #self.editor.setDocument(self.highlighter.document())

app = QApplication(sys.argv)
ex = Main()
ex.show()
sys.exit(app.exec())
