<template>
  <div>
    <a-drawer
      title="容器日志"
      :width="1000"
      :visible="visible"
      :body-style="{ paddingBottom: '80px' }"
      @close="onClose"
    >
    <div v-html="this.log" style="background-color: black; color: white;"></div>
    </a-drawer>
  </div>
</template>

<script>

import {default as AnsiUp} from 'ansi_up';
import {getPodLog} from '@/services/container';
export default {
  name: 'ContainerLog',
  components: {},
  data () {
    return {
      spinning: false,
      advanced: false,
      visible:false,
      log:'',
      form: {
        appId:0,
        envId:0,
        podName:''
      },
    }
  },
  methods: {
    initData(envId,appId,podName){
      this.form.appId = appId
      this.form.envId=envId
      this.form.podName=podName
      this.log=''
      this.getPodLog(this.form)
    },
    getPodLog(params){
      getPodLog(params).then(this.afterGetPodLog)  
    },
    afterGetPodLog(res){
      const data = res.data
      if(data.code == '0'){
        let ansi_up = new AnsiUp()
        data.data.forEach(e=>{
            this.log += ansi_up.ansi_to_html(e) + '<br />'
         })
      }else{
            this.$message.error(data.message)
        }
    },

    showDrawer() {
      this.visible = true;
    },
    onClose() {
      this.visible = false;
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
