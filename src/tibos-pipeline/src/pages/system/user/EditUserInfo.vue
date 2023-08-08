<template>
  <div>
    <a-drawer
      title="修改信息"
      :width="480"
      :visible="visible"
      :body-style="{ paddingBottom: '80px' }"
      @close="onClose"
    >

     <a-form-model ref="ruleForm" :model="form" :label-col="labelCol" :wrapper-col="wrapperCol" :rules="rules">
        <a-form-model-item ref="account" label="账号" prop="account">
          <a-input
            v-model="form.account"
            @blur="
              () => {
                $refs.account.onFieldBlur();
              }
            "
          />
        </a-form-model-item>
        <a-form-model-item ref="pwd" label="密码" prop="pwd">
          <a-input-password 
            v-model="form.pwd"
            @blur="
              () => {
                $refs.pwd.onFieldBlur();
              }
            "
          />
        <password-strength :password="form.pwd" @change="setStrength"></password-strength>
        </a-form-model-item>

        <a-form-model-item ref="confirmPwd" label="确认密码" prop="confirmPwd">
          <a-input-password 
            v-model="form.confirmPwd"
            @blur="
              () => {
                $refs.confirmPwd.onFieldBlur();
              }
            "
          />
        <password-strength :password="form.confirmPwd"></password-strength>
        </a-form-model-item>

        <a-form-model-item ref="phone" label="手机号" prop="phone">
          <a-input
            v-model="form.phone"
          />
        </a-form-model-item>

        <a-form-model-item label="角色组" prop='roles'>
            <a-select  
                      v-model="form.roles"                 
                      mode="multiple" 
                      placeholder="请选择角色组"
            >
            <a-select-option v-for="item in this.roleList" :key="item.enumName">
            {{item.enumName}} -
            {{ item.desction }}
            </a-select-option>
          </a-select>
        </a-form-model-item>
      </a-form-model>
       <div
        :style="{
          position: 'absolute',
          right: 0,
          bottom: 0,
          wuserIdth: '100%',
          borderTop: '1px soluserId #e9e9e9',
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
import PasswordStrength from '@/components/pwd/PasswordStrength'
import {updateUserInfo,getRoles,getUserLogin} from '@/services/user';
export default {
  name: 'EditUserInfo',
  components: {PasswordStrength},
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
      strength:0,
      form: {
        userId: '',
        account: '',
        roles:'',
        pwd:'',
        confirmPwd:''
      },
      roleList:[],
      rules:{
        account: [
          { required: true, message: '请输入账号', trigger: 'blur' },
        ],
        pwd: [
          { required: true, message: '请输入密码', trigger: 'blur' },
          { validator: this.pwdStrengthPass, tirgger: 'blur'}
        ],
        confirmPwd: [
          { required: true, message: '请再次密码', trigger: 'blur' },
          { validator: this.validatePass, tirgger: 'blur'}
        ]
      },
    }
  },
  created:function()
  {
      
  },
  methods: {
    setStrength(strength){
      this.strength = strength
    },
    pwdStrengthPass(rule,value,callback){
      if(this.strength<2){
        callback(new Error("密码难度太低!"));
      }else{
      callback();
      }
    },
    validatePass(rule, value, callback){
          if (value != this.form.pwd) {
              callback(new Error("两次输入密码不一致!"));
          } else {
              callback();
          }
        },
    initData(userId){
      this.form.userId = userId

      getUserLogin({userId:userId}).then(this.afterGetUserLogin)  
      this.getRoles()

    },
    showDrawer() {
      this.visible = true;
    },
    onClose() {
      this.visible = false;
    },
    getRoles(){
      getRoles().then(this.afterGetRoles)  
    },
    afterGetRoles(res){
      const data = res.data
      if(data.code == '0'){
        this.roleList = data.data
      }else{
            this.$message.error(data.message)
        }
    },
    afterGetUserLogin(res){
      const data = res.data
      if(data.code == '0'){
        this.form = data.data
        console.log(this.form)
        if(data.data.roles !=null && data.data.roles != ''){
          this.form.roles = data.data.roles.split(',');
        }else{
          this.form.roles =[]
        }
      }else{
            this.$message.error(data.message)
        }
    },
    onSubmit () {
      this.$refs.ruleForm.validate(async valuserId => {
        if (valuserId) {
          let res = await updateUserInfo(this.form)
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
  @media screen and (max-wuserIdth: 900px) {
    .fold {
      width: 100%;
    }
  }
</style>
