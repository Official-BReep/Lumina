import sys
import time

from PyQt5.QtCore import Qt, QSettings, QDir
from PyQt5.QtGui import QTextCharFormat, QFont, QFontDatabase, QPalette, QColor
from PyQt5.QtWidgets import QApplication, QMainWindow, QVBoxLayout, QPlainTextEdit, QMenu, QHBoxLayout, QWidget, \
    QFileSystemModel, QLabel, QPushButton
from highlighter import Highlighter

class Color(QWidget):

    def __init__(self, color):
        super(Color, self).__init__()
        self.setAutoFillBackground(True)

        palette = self.palette()
        palette.setColor(QPalette.Window, QColor(color))
        self.setPalette(palette)

class IDE(QMainWindow):
    def __init__(self):
        super(IDE, self).__init__()

        self.resize(500,500)
        self.getSettingValues()

        self.setWindowTitle("IDE")

        self.label = QLabel("Projekt Folder")
        self.btn = QPushButton("Klick Mich")

        self.layouts()

        self.highlighter = Highlighter()
        self.setup_editor()

        self.codevalue = self.settings.value("code")

        self.vlayout.addWidget(self.btn)
        self.vlayout.addWidget(self.label)
        self.vlayout2.addWidget(self.editor)

        self.hlayout.addLayout(self.vlayout)
        self.hlayout.addLayout(self.vlayout2)

        self.editor.setPlainText(self.codevalue)

        self.widget = QWidget()
        self.widget.setLayout(self.hlayout)
        self.setCentralWidget(self.widget) # Hinzufügen des Editors zum zentralen Widget des MainWindow

    def layouts(self):
        self.hlayout = QHBoxLayout()
        self.vlayout = QVBoxLayout()
        self.vlayout2 = QVBoxLayout()

        self.hlayout.setContentsMargins(0,0,0,0)
        #self.hlayout.setSpacing(190)
        
    def getSettingValues(self):
        self.settings = QSettings("IDE", 'App')
        try:
            self.resize(self.settings.value('size'))
        except:
            pass

    def closeEvent(self,event):
        self.settings.setValue('size', self.size())
        self.settings.setValue('code', self.editor.toPlainText())


    def setup_editor(self):
        class_format = QTextCharFormat()
        class_format.setForeground(Qt.blue)
        class_format.setFontWeight(QFont.Bold)
        class_pattern = r'^\s*class\s+\w+\(.*$'
        self.highlighter.add_mapping(class_pattern, class_format)

        output_format = QTextCharFormat()
        output_format.setForeground(Qt.GlobalColor.yellow)
        output_format.setFontWeight(QFont.Weight.Bold)
        output_pattern = r'^\s*|w*|.*out\s*\(\"*\.*|w*\"*\)$'
        self.highlighter.add_mapping(output_pattern, output_format)

        define_format = QTextCharFormat()
        define_format.setForeground(Qt.GlobalColor.blue)
        define_format.setFontWeight(QFont.Weight.Bold)
        define_pattern = r'^\s*|w*|.*define\s*\(\"*\.*|w*\"*\)$'
        self.highlighter.add_mapping(define_pattern, define_format)

        output_text_format = QTextCharFormat()
        output_text_format.setForeground(Qt.GlobalColor.black)
        output_text_pattern = r'\(\w*\)$'
        self.highlighter.add_mapping(output_text_pattern, output_text_format)


        string_format = QTextCharFormat()
        string_format.setForeground(Qt.darkGreen)
        string_pattern = r'"\w*\"\,*|;*$'
        self.highlighter.add_mapping(string_pattern, string_format)

        comment_format = QTextCharFormat()
        comment_format.setForeground(Qt.darkGray)
        comment_pattern = r'\b\B!!\s*\W'
        self.highlighter.add_mapping(comment_pattern, comment_format)


        link_format = QTextCharFormat()
        link_format.setForeground(Qt.blue)
        pattern = r'^(?:https?://)?(?:[\w]+\.)(?:\.?[\w]{2,})+$'
        self.highlighter.add_mapping(pattern, link_format)


        self.editor = QPlainTextEdit()
        self.editor.textChanged.connect(self.brackets)

        font = QFontDatabase.systemFont(QFontDatabase.SystemFont.FixedFont)
        self.editor.setFont(font)

        self.highlighter.setDocument(self.editor.document())

    def brackets(self):
        try:
            p = self.editor.textCursor().position()
            result = self.editor.toPlainText()[:p][-1]
            check = self.editor.toPlainText()[:p+1][-1]
            if result == "(":self.editor.insertPlainText(")")
            if result == "{":self.editor.insertPlainText("}")
            if result == "[":self.editor.insertPlainText("]")
            if result == '"':self.editor.insertPlainText('"')
            if result == "'":self.editor.inseltPlainText("'")
            else:pass
        except:pass

    def contextMenuEvent(self,event):
        contextmenu = QMenu(self)
        newAct = contextmenu.addAction("New")
        openAct = contextmenu.addAction("Open")
        quitAct = contextmenu.addAction("Quit")

        action = contextmenu.exec_(self.mapToGlobal(event.pos()))

        if action == quitAct:
            self.close()
        if action == newAct():
            self.editor.document().setPlainText("a")

app = QApplication(sys.argv)
ex = IDE()
ex.show()
sys.exit(app.exec())
