<template>
  
  <a-config-provider :locale="locale" :get-popup-container="popContainer">
    <router-view v-if="isRouterAlive" />
  </a-config-provider>
</template>

<script>
import {enquireScreen} from './utils/util'
import {mapState, mapMutations} from 'vuex'
import themeUtil from '@/utils/themeUtil';
import {getI18nKey} from '@/utils/routerUtil'

export default {
  name: 'App',
  data() {
    return {
      isRouterAlive: true,
      locale: {}
    }
  },
  created () {
    this.setHtmlTitle()
    this.setLanguage(this.lang)
    enquireScreen(isMobile => this.setDevice(isMobile))
  },
  mounted() {
   this.setWeekModeTheme(this.weekMode)
  },
  watch: {
    weekMode(val) {
      this.setWeekModeTheme(val)
    },
    lang(val) {
      this.setLanguage(val)
      this.setHtmlTitle()
    },
    $route() {
      //console.log('route', this.$route.query);
      //if(this.$route.query){
        // if(this.$route.query.appId){
        //   this.setApp(this.$route.query.appId)//将路由中的参数设置到vuex中，调用接口的时候可以从vuex中获取
        // }
        // if(this.$route.query.envId){
        //   this.setEnv(this.$route.query.envId)//将路由中的参数设置到vuex中，调用接口的时候可以从vuex中获取
        // }
      //}
      this.setHtmlTitle()
    },
    appId(newVal) {
      console.log('app改变了', newVal);
      this.$nextTick(() => {
        this.reload();
      }); 
    },
    envId(newVal) {
      console.log('env改变了', newVal);
      this.$nextTick(() => {
        this.reload();
      }); 
    },
    'theme.mode': function(val) {
      let closeMessage = this.$message.loading(`您选择了主题模式 ${val}, 正在切换...`)
      themeUtil.changeThemeColor(this.theme.color, val).then(closeMessage)
    },
    'theme.color': function(val) {
      let closeMessage = this.$message.loading(`您选择了主题色 ${val}, 正在切换...`)
      themeUtil.changeThemeColor(val, this.theme.mode).then(closeMessage)
    },
    'layout': function() {
      window.dispatchEvent(new Event('resize'))
    }
  },
  computed: {
    ...mapState('setting', ['layout', 'theme', 'weekMode', 'lang','app','env']),
    ...mapState('account', ['appId','envId'])
  },
  methods: {
    ...mapMutations('setting', ['setDevice']),
    reload() {
      this.isRouterAlive = false;
      console.log('刷新当前页面了');
      this.$nextTick(() => {
        this.isRouterAlive = true;
      });
    },
    setWeekModeTheme(weekMode) {
      if (weekMode) {
        document.body.classList.add('week-mode')
      } else {
        document.body.classList.remove('week-mode')
      }
    },
    setLanguage(lang) {
      this.$i18n.locale = lang
      switch (lang) {
        case 'CN':
          this.locale = require('ant-design-vue/es/locale-provider/zh_CN').default
          break
        case 'HK':
          this.locale = require('ant-design-vue/es/locale-provider/zh_TW').default
          break
        case 'US':
        default:
          this.locale = require('ant-design-vue/es/locale-provider/en_US').default
          break
      }
    },
    setHtmlTitle() {
      const route = this.$route
      const key = route.path === '/' ? 'home.name' : getI18nKey(route.matched[route.matched.length - 1].path)
      document.title = process.env.VUE_APP_NAME + ' | ' + this.$t(key)
    },
    popContainer() {
      return document.getElementById("popContainer")
    }
  }
}
</script>

<style lang="less" scoped>
  #id{
  }
</style>
