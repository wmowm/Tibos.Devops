<template>
  <a-spin :spinning="spinning">
  <common-layout>
    <div class="top">
      <div class="header">
        <img alt="logo" class="logo" src="@/assets/img/logo.png" />
        <span class="title">{{systemName}}</span>
      </div>
      <div class="desc">gitlab流水线</div>
    </div>
    <div class="login">
      <a-form @submit="onSubmit" :form="form">
        <a-tabs size="large" :tabBarStyle="{textAlign: 'center'}" style="padding: 0 2px;">
          <a-tab-pane tab="账户密码登录" key="1">
            <a-alert type="error" :closable="true" v-show="error" :message="error" showIcon style="margin-bottom: 24px;" />
            <a-form-item>
              <a-input
                autocomplete="autocomplete"
                size="large"
                placeholder="请输入账号"
                v-decorator="['account', {initialValue :account, rules: [{ required: true, message: '请输入账户', whitespace: true}]}]"
              >
                <a-icon slot="prefix" type="user" />
              </a-input>
            </a-form-item>
            <a-form-item>
              <a-input
                size="large"
                placeholder="请输入密码"
                autocomplete="autocomplete"
                type="password"
                v-decorator="['pwd', {initialValue :pwd,rules: [{ required: true, message: '请输入密码', whitespace: true}]}]"
              >
                <a-icon slot="prefix" type="lock" />
              </a-input>
            </a-form-item>
          </a-tab-pane>
        </a-tabs>
        <a-form-item>
          <a-button :loading="spinning" style="width: 100%;margin-top: 24px" size="large" htmlType="submit" type="primary">登录</a-button>
        </a-form-item>
        <div>
          其他登录方式
          <a-icon class="icon" type="gitlab" @click="getAuthorize" />
          <router-link style="float: right" to="" onclick="javascript:window.location.href='http://gitlab.wmowm.com:880/users/sign_in'" >注册账户</router-link>

          
        </div>
      </a-form>
    </div>
  </common-layout>
</a-spin>
</template>

<script>
import CommonLayout from '@/layouts/CommonLayout'
import {login, getRoutesConfig,getFavoriteAppList} from '@/services/user'
import {getEnvList} from '@/services/env'
import {setAuthorization} from '@/utils/request'
import {loadRoutes} from '@/utils/routerUtil'
import {mapMutations} from 'vuex'
import {getAuthorize,gitlabLogin} from '@/services/user';

export default {
  name: 'Login',
  components: {CommonLayout},
  data () {
    return {
      spinning:false,
      account:"",
      pwd:"",
      error: '',
      form: this.$form.createForm(this),
    }
  },
  computed: {
    systemName () {
      return this.$store.state.setting.systemName
    }
  },
  created:function()
  {
      const searchParams = new URLSearchParams(window.location.search);
      const code = searchParams.get("code");
      
      if(code != '' && code != null){
        this.spinning = true
        gitlabLogin({code}).then(this.afterLogin)
      }
  },
  methods: {
    ...mapMutations('account', ['setUser', 'setPermissions', 'setRoles','setMyAppList','setMyEnvList']),
    getAuthorize(e){
      e.preventDefault()
        getAuthorize().then(this.afterGetAuthorize)    
    },
    getFavoriteAppList(){
      getFavoriteAppList().then(this.afterGetFavoriteAppList)  
    },
    afterGetFavoriteAppList(res){
      const data = res.data
      if(data.code == '0'){
        this.setMyAppList(data.data)
      }else{
            this.$message.error(data.message)
        }
    },
    getEnvList(){
        this.spinning = true
        getEnvList({pageIndex:1,pageSize:1000}).then(this.afterGetEnvList)    
    },
    afterGetEnvList(res){
        this.spinning = false
        const data = res.data
        if(data.code == "0"){
          this.setMyEnvList(data.data)
        }else {
            this.$message.error(data.message)
        }
    },
    onSubmit (e) {
      e.preventDefault()
      this.form.validateFields((err) => {
        if (!err) {
          this.spinning = true
          const account = this.form.getFieldValue('account')
          const pwd = this.form.getFieldValue('pwd')
          login({account:account,pwd:pwd}).then(this.afterLogin)
        }
      })
    },
    afterLogin(res) {
      this.spinning = false
      const loginRes = res.data
      if (loginRes.code == "0") {
        const {user, permissions} = loginRes.data
        this.setUser(user)
        this.setPermissions(permissions)
        //this.setRoles(roles)
        setAuthorization({token: loginRes.data.token, expireAt: new Date(loginRes.data.expireAt)})
        //收藏的应用
        this.getFavoriteAppList()
        //环境配置
        this.getEnvList()
        // 获取路由配置
        getRoutesConfig().then(result => {
          const routesConfig = result.data.data
          loadRoutes(routesConfig)
          this.$router.push('/dashboard/workplace')
          this.$message.success(loginRes.message, 3)
        })
      } else {
        this.error = loginRes.message
      }
    },
    afterGetAuthorize(res){
        const data = res.data
        if(data.code == "0"){
            console.log(data.data)
            window.location.href =data.data
        }else {
            this.$message.error(data.message)
        }
    }
  }
}
</script>

<style lang="less" scoped>
  .common-layout{
    .top {
      text-align: center;
      .header {
        height: 44px;
        line-height: 44px;
        a {
          text-decoration: none;
        }
        .logo {
          height: 44px;
          vertical-align: top;
          margin-right: 16px;
        }
        .title {
          font-size: 33px;
          color: @title-color;
          font-family: 'Myriad Pro', 'Helvetica Neue', Arial, Helvetica, sans-serif;
          font-weight: 600;
          position: relative;
          top: 2px;
        }
      }
      .desc {
        font-size: 14px;
        color: @text-color-second;
        margin-top: 12px;
        margin-bottom: 40px;
      }
    }
    .login{
      width: 368px;
      margin: 0 auto;
      @media screen and (max-width: 576px) {
        width: 95%;
      }
      @media screen and (max-width: 320px) {
        .captcha-button{
          font-size: 14px;
        }
      }
      .icon {
        font-size: 24px;
        color: @text-color-second;
        margin-left: 16px;
        vertical-align: middle;
        cursor: pointer;
        transition: color 0.3s;

        &:hover {
          color: @primary-color;
        }
      }
    }
  }
</style>
