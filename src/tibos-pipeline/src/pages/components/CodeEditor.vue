<template>
    <div class="json-editor">
      <textarea ref="textarea"/>
    </div>
  </template>
  
  <script>
  import CodeMirror from 'codemirror'
  import 'codemirror/addon/lint/lint.css'
  import 'codemirror/lib/codemirror.css'
  import 'codemirror/theme/panda-syntax.css'
  import 'codemirror/mode/javascript/javascript'
  import 'codemirror/addon/lint/json-lint'
  // 折叠代码
  import 'codemirror/addon/fold/foldgutter.css';
  import 'codemirror/addon/fold/foldcode.js';
  import 'codemirror/addon/fold/foldgutter.js';
  import 'codemirror/addon/fold/brace-fold.js';
  import 'codemirror/addon/fold/xml-fold.js';
  import 'codemirror/addon/fold/indent-fold.js';
  import 'codemirror/addon/fold/markdown-fold.js';
  import 'codemirror/addon/fold/comment-fold.js';
  
  export default {
    name: 'CodeEditor',
    props: ['value'],
    data () {
      return {
        codeEditor: false
      }
    },
    watch: {
      value (value) {
        const editor_value = this.codeEditor.getValue()
        if (value !== editor_value) {
          this.codeEditor.setValue(value)
        }
      }
    },
    mounted () {
      this.codeEditor = CodeMirror.fromTextArea(this.$refs.textarea, {
        lineNumbers: true,
        mode: 'text/javascript',
        gutters: ['CodeMirror-lint-markers',"CodeMirror-linenumbers","CodeMirror-foldgutter"],
        theme: 'panda-syntax',
        lint: true,
        foldGutter: {
          rangeFinder: new CodeMirror.fold.combine(CodeMirror.fold.indent,CodeMirror.fold.comment)
        },
      })

      this.codeEditor.setValue(this.value)
      this.codeEditor.on('change', cm => {
        this.$emit('changed', cm.getValue())
        this.$emit('input', cm.getValue())
      })
    },
    methods: {
      getValue () {
        return this.codeEditor.getValue()
      }
    }
  }
  </script>
  
  <style scoped>
  .json-editor {
    height: 100%;
    position: relative;
  }
  .json-editor >>> .CodeMirror {
    height: auto;
    min-height: 180px;
  }
  .json-editor >>> .CodeMirror-scroll {
    min-height: 180px;
  }
  .json-editor >>> .cm-s-rubyblue span.cm-string {
    color: #f08047;
  }
  </style>
  