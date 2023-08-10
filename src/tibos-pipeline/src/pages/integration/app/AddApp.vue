<template>
  <div>
    <a-drawer
      title="创建应用"
      :width="600"
      :visible="visible"
      :body-style="{ paddingBottom: '80px' }"
      @close="onClose"
    >
     <a-form-model ref="ruleForm" :model="form" :label-col="labelCol" :wrapper-col="wrapperCol" :rules="rules">
      <a-form-model-item
              ref="projectId"
              label="所属项目"
              prop="projectId"
            >
              <a-select
                :default-value="form.projectId"
                v-model="form.projectId"
                disabled
              >
                <a-select-option :value="form.projectId">
                  {{form.projectName}}
                </a-select-option>
              </a-select>
          </a-form-model-item>


        <a-form-model-item ref="name" label="应用名称" prop="name">
          <a-input
            v-model="form.name"
            @blur="
              () => {
                $refs.name.onFieldBlur();
              }
            "
          />
        </a-form-model-item>

        <a-form-model-item label="组" prop='group'>
            <a-select  
                      v-model="form.group"                 
                      placeholder="请选择组"
                      @change="handleChange"
            >
            <a-select-option v-for="item in this.groupList" :key="item">
            {{item}}
            </a-select-option>
          </a-select>
        </a-form-model-item>

        <a-form-model-item label="模板" prop='templateId'>
            <a-select  
                      v-model="form.templateId"                 
                      placeholder="请选择模板"
                      @change="handleChange"
            >
            <a-select-option v-for="item in this.templateList" :key="item.id">
            {{item.name}}
            </a-select-option>
          </a-select>
        </a-form-model-item>

        <a-form-model-item ref="remark" label="描述" prop="remark">
          <a-textarea
          placeholder="" :default-value="form.remark" v-model="form.remark"
          :auto-size="{ minRows: 2, maxRows: 6 }"
        />
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
        <a-button :loading="spinning" type="primary" @click="onSubmit">
          提交
        </a-button>
      </div>

    </a-drawer>
  </div>
</template>

<script>

import {createApp,} from '@/services/app';
import {getUserInfo} from '@/services/user';
import {getTemplateList} from '@/services/template';
export default {
  name: 'AddApp',
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
      groupList:[],
      templateList:[],
      form: {
        name: '',
        remark:'',
        projectId:'',
        projectName:'',
        group:'',
        templateId:''
      },
      rules:{
        name: [
          { required: true, message: '请输入应用名称', trigger: 'blur' },
          { validator: this.validateName, tirgger: 'blur'}
        ],
        projectId: [
          { required: true, message: '请选择项目', trigger: 'blur' },
        ],
        group: [
          { required: true, message: '请选择组', trigger: 'blur' },
        ],
        templateId: [
          { required: true, message: '请选择模板', trigger: 'blur' },
        ]
      },
    }
  },
  created:function()
  {
      
  },
  methods: {
    validateName(rule, value, callback){
    const reg = /^[a-zA-Z0-9._]*$/g;
    const bl = reg.test(value)
    if (bl) {
        callback();
      } else {
        callback(new Error("应用名称只能由数字字母,下划线,点组成!"));
      }
    },
    initData(projectId,projectName){
      this.form.name = ''
      this.form.remark = ''
      this.form.projectId=projectId
      this.form.projectName=projectName
      this.form.group=''
      this.form.templateId=''
      this.getUserInfo()
      this.getTemplateList({pageIndex:1,pageSize:100000})
    },
    getUserInfo(){
      getUserInfo().then(this.afterGetUserInfo)  
    },
    afterGetUserInfo(res){
      const data = res.data
      if(data.code == '0'){
        console.log(data.data.group)
        if(data.data.group ==null || data.data.group =='' || data.data.group == undefined){
          this.groupList =[]
        }else{
          this.groupList = data.data.group.split(',')
        }
      }else{
            this.$message.error(data.message)
        }
    },
    getTemplateList(params){
        this.spinning = true
        getTemplateList(params).then(this.afterGetTemplateList)    
    },
    afterGetTemplateList(res){
        this.spinning = false
        const data = res.data
        if(data.code == "0"){
            this.templateList = data.data
        }else {
            this.$message.error(data.message)
        }
    },
    showDrawer() {
      this.visible = true;
    },
    onClose() {
      this.visible = false;
    },
    onSubmit () {
      this.$refs.ruleForm.validate(async valid => {
        if (valid) {
          this.spinning = true
          let res = await createApp(this.form)
          const data = res.data;
          if (data.code === '0') {
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
    handleChange(){

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
