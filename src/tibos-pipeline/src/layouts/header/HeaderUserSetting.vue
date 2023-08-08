<template>
<div>
    <a-modal v-model:visible="visible" title="用户设置">
      <a-form-model ref="ruleForm" :model="form" :label-col="labelCol" :wrapper-col="wrapperCol" :rules="rules">
        <a-form-model-item ref="oldPwd" label="旧密码" prop="oldPwd">
          <a-input-password
            v-model="form.oldPwd"
            @blur="
              () => {
                $refs.oldPwd.onFieldBlur();
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
    </a-modal>
  </div>
</template>


<script>
import {updateUserPwd,logout} from '@/services/user'
import PasswordStrength from '@/components/pwd/PasswordStrength'
import {mapGetters} from 'vuex'
export default {
  name: 'HeaderUserSetting',
  components: {PasswordStrength},
  data(){
    return{
      spinning: false,
      advanced: false,
      visible: false,
      labelCol: { span: 4 },
      wrapperCol: { span: 20 },
      fileList: [],
      uploading: false,
      strength:0,
      form: {
        oldPwd: '',
        pwd:'',
        confirmPwd:''
      },
      rules:{
        oldPwd: [
          { required: true, message: '请输入旧密码', trigger: 'blur' },
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
  computed: {
    ...mapGetters('account', ['user']),
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
    onClose() {
      this.visible = false;
    },
    onSubmit(){
      this.$refs.ruleForm.validate(async valuserId => {
        if (valuserId) {
          let res = await updateUserPwd(this.form)
          const data = res.data;
          if (data.code === '0') {
            this.uploading = false
            this.$message.success('操作成功!');
            this.$emit('refresh');
            this.onClose()
            logout()
            this.$router.push('/login')
          } else {
            this.uploading = false
            this.$message.error(data.message)
          }
        } else {
          return false
        }
      })
    },
  }
}
</script>

<style lang="less">
  .header-avatar{
    display: inline-flex;
    .avatar, .name{
      align-self: center;
    }
    .avatar{
      margin-right: 8px;
    }
    .name{
      font-weight: 500;
    }
  }
  .avatar-menu{
    width: 150px;
  }

</style>
