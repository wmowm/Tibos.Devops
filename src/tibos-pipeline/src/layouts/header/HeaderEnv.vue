<template>
  <div class="header-env">
  <a-radio-group v-model:value="value" @change="(val) => setEnvId(val.target.value)"
    :selected-key="[value]">
   <a-radio 
      v-for="item in envList" 
      :key="item.id"
      :value="item.id"
      >
      {{ item.name }}
   </a-radio>
  </a-radio-group>
</div>
</template>

<script>

import { mapMutations ,mapGetters} from 'vuex';
// import store from '../../store'
export default {
  name: 'HeaderEnv',
  data(){
    return {
      value: '',
      envList:[]
    }
  },
  created () {
    this.envList =this.myEnvList()
    const envId =this.envId()
    if(envId =='' || envId == null || envId == undefined){
      this.value = this.envList[0].id
    }else{
      this.value = envId
    }
    this.setEnvId(this.value)
  },
//   computed: {
//     ...mapState('setting', [
//       'env',
//     ]),
// },
  methods: {
    ...mapMutations('account', ['setEnvId']),
    ...mapGetters('account', ['myEnvList','envId']),
  },
  watch:{
		'$store.state.account.myEnvList'(newVal){
			//对数据执行操作
			this.envList =newVal
      const envId =this.envId()
      if(envId ==''){
        this.value = this.envList[0].id
      }
		}
	},
}
</script>

<style lang="less">
  .header-env{
    margin-left: 200px;
    display: inline-flex;
  }
  .env-menu{
    width: 150px;
  }
  .rate{
    color:#1890ff
  }
</style>
