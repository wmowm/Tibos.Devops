<template>
  <div>
    <a-drawer
      :title="actionType == 0 ? `新增模板`: `编辑模板` "
      :width="720"
      :visible="visible"
      :body-style="{ paddingBottom: '80px' }"
      @close="onClose"
    >

     <a-form-model ref="ruleForm" :model="form" :label-col="labelCol" :wrapper-col="wrapperCol" :rules="rules">
        <a-form-model-item ref="name" label="模板名称" prop="name">
          <a-input
            v-model="form.name"
            @blur="
              () => {
                $refs.name.onFieldBlur();
              }
            "
          />
        </a-form-model-item>
        <a-form-model-item ref="name" label="占位变量" prop="tempVal">
          <a-input
            v-model="form.tempVal"
            @blur="
              () => {
                $refs.tempVal.onFieldBlur();
              }
            "
          />
        </a-form-model-item>
        <a-form-model-item ref="remark" label="描述" prop="remark">
          <a-textarea
          placeholder="" :default-value="form.remark" v-model="form.remark"
          :auto-size="{ minRows: 2, maxRows: 6 }"
        />
        </a-form-model-item>

        <a-form-model-item  label="文件选择">
          <a-upload-dragger :before-upload="beforeUpload" :remove="handleRemove" :multiple="false" :file-list="fileList">
            <p class="ant-upload-drag-icon">
              <a-icon type="inbox" />
            </p>
            <p class="ant-upload-text">
              点击上传文件
            </p>
            <p class="ant-upload-hint">
              支持扩展名：.zip为后缀名的文件
            </p>
         </a-upload-dragger>
        </a-form-model-item>
        <a-form-model-item label="导入说明" prop="faq" has-feedback>
          <p>1、请将模板文件夹压缩成zip格式</p>
          <p>2、zip文件名称只能为字母跟数字[a-z][A-Z][0-9]</p>
          <p>3、压缩的文件夹名称必须与zip文件名称一致</p>
          <p>4、占位变量为模板文件全局替换的变量,区分大小</p>
        </a-form-model-item>
      </a-form-model>
       <div
        :style="{
          position: 'absolute',
          right: 0,
          bottom: 0,
          width: '100%',
          borderTop: '1px solid #e9e9e9',
          padding: '10px 16px',
          background: '#fff',
          textAlign: 'right',
          zIndex: 1,
        }"
      >
        <a-button :style="{ marginRight: '8px' }" @click="onClose">
          取消
        </a-button>
        <a-button type="primary" @click="onSubmit">
          提交
        </a-button>
      </div>


    </a-drawer>
  </div>
</template>

<script>

import {getTemplateById,createTemplate,updateTemplate} from '@/services/template';
export default {
  name: 'AddOrEditTemplate',
  components: {},
  data () {
    return {
      actionType: 0,
      spinning: false,
      advanced: false,
      visible: false,
      labelCol: { span: 6 },
      wrapperCol: { span: 18 },
      fileList: [],
      form: {
        id: '',
        name:'',
        tempVal:'',
        remark:'',
      },
      rules:{
        name: [
          { required: true, message: '请输入模板名称', trigger: 'blur' },
        ],
        tempVal: [
          { required: true, message: '请输入占位变量', trigger: 'blur' },
          { validator: this.validateTempVal, tirgger: 'blur'}
        ],
      },
    }
  },
  created:function()
  {
      
  },
  methods: {
    validateTempVal(rule, value, callback){
    const reg = /^[a-zA-Z0-9]*$/g;
    const bl = reg.test(value)
    if (bl) {
        callback();
      } else {
        callback(new Error("占位变量只能由数字字母组成!"));
      }
    },
    initData(id){
      this.form.id = id
      this.fileList =[]
      if(id=='' || id==null || id== undefined){
      this.form.id=''
      this.form.name = ''
      this.form.remark = ''
      this.form.tempVal = ''
      }else{
        getTemplateById({id:id}).then(this.afterGetTemplateById)  
      }
    },
    showDrawer() {
      this.visible = true;
    },
    onClose() {
      this.visible = false;
    },
    afterGetTemplateById(res){
      const data = res.data
      if(data.code == '0'){
        this.form = data.data
      }else{
            this.$message.error(data.message)
        }
    },
    // 文件移除
    handleRemove (file) {
      const index = this.fileList.indexOf(file)
      const newFileList = this.fileList.slice()
      newFileList.splice(index, 1)
      this.fileList = newFileList
    },
    beforeUpload (file) {
      this.fileList = [...this.fileList, file]
      return false
    },
    onSubmit () {
      this.$refs.ruleForm.validate(async valid => {
        if (valid) {
          const { fileList } = this
          const formData = new FormData()
          fileList.forEach((file) => {
            formData.append('file', file)
          })

          for (const prop in this.form) {
            if (Object.prototype.hasOwnProperty.call(this.form, prop)) {
              formData.append(prop, this.form[prop])
            }
          }
          this.spinning = true
          let res = {}
          if(this.actionType == 0){
              res = await createTemplate(formData)
          }else{
              res = await updateTemplate(formData)
          }
          const data = res.data;
          if (data.code === '0') {
            this.fileList = []
            this.spinning = false
            this.$message.success('操作成功!');
            this.$emit('refresh');
            this.onClose()

          } else {
            this.spinning = false
            this.$message.error(data.message)
          }
        } else {
          return false
        }
      })
    },
    toggleAdvanced () {
      this.advanced = !this.advanced
    },
  }
}
</script>

<style lang="less" scoped>
  .search{
    margin-bottom: 54px;
  }
  .fold{
    width: calc(100% - 216px);
    display: inline-block
  }
  .operator{
    margin-bottom: 18px;
  }
  @media screen and (max-width: 900px) {
    .fold {
      width: 100%;
    }
  }
</style>
