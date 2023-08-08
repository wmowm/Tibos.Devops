<template>
  <a-card>
    <div>
      <a-space >
        <a-button @click="createConfigMap" type="primary">添加配置</a-button>
        <a-popconfirm
          title="确定重载?"
          okText="确定"
          @confirm="() => this.redeployment()"
        >
          <a-button type="danger">重载配置</a-button>
        </a-popconfirm>
        <!-- <a-button  >系统变量</a-button> -->
      </a-space>
    <a-spin :spinning="spinning">
      <div style="background-color: #ececec; padding: 20px;margin-top: 20px;">
        <a-row :gutter="16">
          <a-col v-for="item in this.data" :key="item.id" :span="6" class="col">
            <a-card :title="item.subPath" :bordered="false">
              <template #extra><a-badge :status="item.status==1?'success':'default'" :text="item.status==1?'已挂载':'未挂载'" /></template>
              <p>{{ item.remark }}</p>
              <template #actions>
                <a-icon type="edit" title="修改配置" @click="updateConfigContent(item.id,item.content,item.restartContainer)"/>
                

                <a-popconfirm
                  :title="item.status?'确定取消挂载?':'确定挂载配置?'"
                  okText="确定"
                  @confirm="() => updateConfigStatus(item.id,!item.status)"
                >
                <a-icon :type="item.status?'thunderbolt':'api'" :title="item.status?'取消挂载':'挂载配置'"/>
                </a-popconfirm>

                <a-popconfirm
                  title="确定删除?"
                  okText="确定"
                  @confirm="() => deleteConfigMap(item.id)"
                >
                <a-icon type="delete" title="删除配置"/>
                </a-popconfirm>
                <a-icon type="setting" title="设置配置" @click="updateConfigMap(item.id)"/>
              </template>
            </a-card>
          </a-col>
        
        </a-row>
        <a-empty v-if="this.data.length==0" />
        
      </div>
    </a-spin>
    <add-or-edit-config-map ref="addOrEditConfigMap" @refresh= "refresh" />
    <edit-config-content ref="editConfigContent" @refresh= "refresh" />
    </div>

  </a-card>
</template>

<script>
import { mapGetters} from 'vuex';
import {getConfigMapList,updateConfigStatus,deleteConfigMap,redeployment} from '@/services/configmap';
import AddOrEditConfigMap from './AddOrEditConfigMap.vue';
import EditConfigContent from './EditConfigContent.vue';
export default {
  name: 'QueryList',
  components: {AddOrEditConfigMap,EditConfigContent},
  data () {
    return {
      advanced: false,
      data: [],
      searchData:{
        envId: 0,
        appId: 0
      },
      form: this.$form.createForm(this, { name: 'search' }),
      pagination: {},
      selectedRows: [],
      spinning:false,
      autoRefresh:true
    }
  },
//   authorize: {
//     deleteRecord: 'delete'
//   },
  created:function()
  {
    this.searchData.appId =this.appId()
    this.searchData.envId =this.envId()
    console.log('当前',this.searchData.appId, this.searchData.envId)
    this.getConfigMapList(this.searchData) 

  },
  methods: {
    ...mapGetters('account', ['appId','envId']),
    getConfigMapList(params){
        this.spinning = true
        getConfigMapList(params).then(this.afterGetConfigMapList)    
    },
    afterGetConfigMapList(res){
        this.spinning = false
        const data = res.data
        if(data.code == "0"){
            this.data = data.data
        }else {
            this.$message.error(data.message)
        }
    },
    createConfigMap(){
      this.$refs.addOrEditConfigMap.actionType = 0
      this.$refs.addOrEditConfigMap.initData('',this.searchData.envId,this.searchData.appId)
      this.$refs.addOrEditConfigMap.showDrawer()
    },
    updateConfigMap(id){
      this.$refs.addOrEditConfigMap.actionType = 1
      this.$refs.addOrEditConfigMap.initData(id,this.searchData.envId,this.searchData.appId)
      this.$refs.addOrEditConfigMap.showDrawer()
    },
    updateConfigContent(id,content,restartContainer){
      this.$refs.editConfigContent.initData(id,content,restartContainer)
      this.$refs.editConfigContent.showDrawer()
    },
    redeployment(){
      this.spinning = true
      redeployment(this.searchData).then(this.afterRedeployment)  
    },
    afterRedeployment(res){
        this.spinning = false
        const data = res.data
        if(data.code == "0"){
            this.$message.success('操作成功!');
            this.refresh()
        }else{
          this.$message.error('操作失败!');
        }
    },
    deleteConfigMap(id){
      this.spinning = true
      deleteConfigMap({id:id}).then(this.afterDeleteConfigMap)    
    },
    afterDeleteConfigMap(res){
        this.spinning = false
        const data = res.data
        if(data.code == "0"){
            this.$message.success('操作成功!');
            this.refresh()
        }else{
          this.$message.error('操作失败!');
        }
    },
    updateConfigStatus(id,status){
      this.spinning = true
      updateConfigStatus({id:id,status:status}).then(this.afterUpdateConfigStatus)    
    },
    afterUpdateConfigStatus(res){
        this.spinning = false
        const data = res.data
        if(data.code == "0"){
            this.$message.success('操作成功!');
            this.refresh()
        }else{
          this.$message.error('操作失败!');
        }
    },
    refresh(){
      this.getConfigMapList(this.searchData)
    },
    },
    toggleAdvanced () {
      this.advanced = !this.advanced
    },
}
</script>

<style lang="less" scoped>
  p {
  white-space: nowrap;  /* 文本强制不换行 */
  overflow: hidden;    /* 限制溢出部分显示 */
  text-overflow: ellipsis;  /* 超出部分显示省略号 */
  }
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

  .col{
    margin-bottom: 10px
  }
</style>
