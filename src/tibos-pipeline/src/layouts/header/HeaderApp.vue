<template>
  <a-dropdown>
    <div class="header-app">
      <span class="name">{{defalutApp.name}}</span>
    </div>
    <a-menu @click="val => setAppId(val.key)"
      :selected-key="defalutApp"
      :class="['app-menu']" slot="overlay">
      <a-menu-item v-for="item in appList" :key="item.id">
        <span>{{item.name}}</span>
        <span style="float: right;" @click="showApp(item.id)">
          <a-icon type="file-search" />
        </span>
      </a-menu-item>
      <a-menu-divider />
    </a-menu>
  </a-dropdown>
</template>

<script>


import {mapMutations, mapGetters} from 'vuex';

export default {
  name: 'HeaderApp',
  data(){
    return {
      id:0,
      appList:[],
      defalutApp:{}
    }
  },
  created () {
    this.appList =this.myAppList()
      if(this.appList != null){
        const appId =this.appId()
        if(appId =='' || appId == null || appId == undefined){
          this.defalutApp = this.appList[0]
        }else{
        this.defalutApp = this.appList.find(item=>item.id==appId)
      }
      console.log(this.defalutApp)
      this.setAppId(this.defalutApp.id)
    }
  },
  // computed: {
  //   ...mapState('setting', [
  //     'app',
  //   ]),
  // },
  methods: {
    ...mapGetters('account', ['myAppList','appId']),
    ...mapMutations('account', ['setAppId']),
    showApp(id){

      this.id = id
      this.$emit('change', this.id)
    },
  },
  watch:{
		'$store.state.account.myAppList'(newVal){
			//对数据执行操作
			this.appList =newVal
      const appId =this.appId()
      if(appId ==''){
        this.defalutApp = this.appList[0]
        this.setAppId(this.defalutApp.id)
      }else{
        const app =this.appList.find(item=>item.id==appId)
        if(app == null){
          //刷新页面
          this.setAppId(this.appList[0].id)
        }else{
          this.defalutApp = app
          this.setAppId(this.defalutApp.id)
        }
      }
		}
	},
}
</script>

<style lang="less">
  .header-app{
    display: inline-flex;
    .app, .name{
      align-self: center;
    }
    .app{
      margin-right: 8px;
    }
    .name{
      font-weight: 500;
    }
  }
  .app-menu{
    width: 150px;
  }

</style>
