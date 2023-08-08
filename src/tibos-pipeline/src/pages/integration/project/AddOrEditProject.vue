<template>
  <div>
    <a-drawer
      :title="actionType == 0 ? `新增项目`: `编辑项目` "
      :width="480"
      :visible="visible"
      :body-style="{ paddingBottom: '80px' }"
      @close="onClose"
    >

     <a-form-model ref="ruleForm" :model="form" :label-col="labelCol" :wrapper-col="wrapperCol" :rules="rules">
        <a-form-model-item ref="name" label="项目名称" prop="name">
          <a-input
            v-model="form.name"
            @blur="
              () => {
                $refs.name.onFieldBlur();
              }
            "
          />
        </a-form-model-item>


        <a-form-model-item label="团队" prop='teamId'>
            <a-select  
                      v-model="form.teamId"                 
                      placeholder="请选择团队"
            >
            <a-select-option v-for="item in this.teamList" :key="item.id">
            {{item.name}}
            </a-select-option>
          </a-select>
        </a-form-model-item>

        <a-form-model-item label="域名" prop='domain'>
            <a-select  
                      v-model="form.domain"                 
                      placeholder="请选择域名"
            >
            <a-select-option v-for="item in this.domainList" :key="item">
            {{item}}
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
        <a-button type="primary" @click="onSubmit">
          提交
        </a-button>
      </div>


    </a-drawer>
  </div>
</template>

<script>

import {getProjectById,createProject,updateProject} from '@/services/project';
import {getUserTeamList} from '@/services/user';
export default {
  name: 'AddOrEditProject',
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
      uploading: false,
      form: {
        id: '',
        name: '',
        remark:'',
        teamId:'',
        domain:''
      },
      teamList:[],
      domainList:[],
      rules:{
        name: [
          { required: true, message: '请输入项目名称', trigger: 'blur' },
        ],
        teamId: [
          { required: true, message: '请选择团队', trigger: 'blur' },
        ],
        domain: [
          { required: true, message: '请选择域名', trigger: 'blur' },
        ]
      },
    }
  },
  created:function()
  {
      this.getUserTeamList()
  },
  methods: {
    initData(id){
      this.form.id = id
      if(id=='' || id==null || id== undefined){
      this.form.id=''
      this.form.name = ''
      this.form.remark = ''
      this.form.teamId=''
      }else{
        getProjectById({id:id}).then(this.afterGetProjectById)  
      }
    },
    showDrawer() {
      this.visible = true;
    },
    onClose() {
      this.visible = false;
    },
    getUserTeamList(params){
        this.spinning = true
        getUserTeamList(params).then(this.afterGetUserTeamList)    
    },
    afterGetUserTeamList(res){
        this.spinning = false
        const data = res.data
        if(data.code == "0"){
            this.teamList = data.data
        }else {
            this.$message.error(data.message)
        }
    },
    afterGetProjectById(res){
      const data = res.data
      if(data.code == '0'){
        this.form = data.data
      }else{
            this.$message.error(data.message)
        }
    },
    onSubmit () {
      this.$refs.ruleForm.validate(async valid => {
        if (valid) {
          let res = {}
          if(this.actionType == 0){
              res = await createProject(this.form)
          }else{
              res = await updateProject(this.form)
          }
          const data = res.data;
          if (data.code === '0') {
            this.uploading = false
            this.$message.success('操作成功!');
            this.$emit('refresh');
            this.onClose()

          } else {
            this.uploading = false
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
  },
  watch:{
    'form.teamId'(id){
      const team = this.teamList.find(p=>{return p.id==id})
      if(team == null || team.domains == null || team.domains == ''){
        this.domainList = []
      }else{
        this.domainList = team.domains.split(',')
      }
    }
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
