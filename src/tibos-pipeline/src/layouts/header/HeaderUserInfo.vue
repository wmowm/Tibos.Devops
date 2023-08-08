<template>
<div>
    <a-modal v-model:visible="visible" title="个人中心" @ok="handleOk">
      <p>敬请期待...</p>
    </a-modal>
  </div>
</template>


<script>
//import {logout} from '@/services/user'

export default {
  name: 'HeaderUserInfo',
  data(){
    return{
      visible:false
    }
  },
  computed: {
    
  },
  methods: {
    handleOk(){

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
