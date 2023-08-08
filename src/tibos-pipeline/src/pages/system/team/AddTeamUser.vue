<template>
  <div>
    <a-drawer
      title="加入团队"
      :width="480"
      :visible="visible"
      :body-style="{ paddingBottom: '80px' }"
      @close="onClose"
    >

     <a-form-model ref="ruleForm" :model="form" :label-col="labelCol" :wrapper-col="wrapperCol" :rules="rules">
        <a-form-model-item
              ref="teamId"
              label="团队"
              prop="teamId"
            >
              <a-select
                :default-value="form.teamId"
                v-model="form.teamId"
                disabled
              >
                <a-select-option :value="form.teamId">
                  {{form.teamName}}
                </a-select-option>
              </a-select>
            </a-form-model-item>

          <a-form-model-item label="用户" prop='userIds'>
            <a-select  
                      v-model="form.userIds"                 
                      mode="multiple" 
                      placeholder="请选择用户"
                      @change="handleChange"
            >
            <a-select-option v-for="item in this.userList" :key="item.id">
              <a-avatar class="avatar" size="small" shape="circle" :src="item.avatarUrl"/>
            {{item.nickName}}
            </a-select-option>
          </a-select>
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

import {joinTeam,getNotJoinUserList} from '@/services/team';
export default {
  name: 'AddTeamUser',
  components: {},
  data () {
    return {
      spinning: false,
      advanced: false,
      visible: false,
      labelCol: { span: 4 },
      wrapperCol: { span: 20 },
      fileList: [],
      uploading: false,
      form: {
        userIds: [],
        teamId:'',
        teamName:''
      },
      userList: [],

      rules:{
      teamId: [
        { required: true, message: '请输入项目名称', trigger: 'blur' },
        ],
        userIds: [
        { required: true, message: '请选择用户', trigger: 'blur' },
        ]
      },
    }
  },
  created:function()
  {
    
  },
  methods: {
    initData(teamId,teamName){
      this.form.teamId = teamId
      this.form.teamName = teamName 
      this.form.userIds =[]
      this.getNotJoinUserList({teamId:this.form.teamId})
    },
    showDrawer() {
      this.visible = true;
    },
    onClose() {
      this.visible = false;
    },
    getNotJoinUserList(params){
        this.spinning = true
        getNotJoinUserList(params).then(this.afterGetNotJoinUserList)    
    },
    afterGetNotJoinUserList(res){
      const data = res.data
      if(data.code == '0'){
        this.userList = data.data
      }else{
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
    handleChange(value){
      console.log(value)
    },
    onSubmit () {
      this.$refs.ruleForm.validate(async valid => {
        if (valid) {
          let res = {}
          res = await joinTeam(this.form)
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
