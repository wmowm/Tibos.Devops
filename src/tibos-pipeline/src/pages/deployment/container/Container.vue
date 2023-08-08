<template>
  <a-card>
    <div>
      <a-space >
        <a-button @click="scalePod" type="primary">伸缩</a-button>
        <a-popconfirm
          title="确定重启?"
          @confirm="() => this.restartPod()"
        >
          <a-button type="danger"  >重启容器</a-button>
        </a-popconfirm>
        <a-button @click="refresh" >刷新容器</a-button>
      </a-space>
    <a-spin :spinning="spinning">
      <standard-table
        :columns="columns"
        :dataSource="data"
        :pagination="pagination"
        :rowSelection=null
        rowKey="name"
      >
        <div slot="message" slot-scope="{text}">
          <div v-if="text.message !=null && text.message != ''">
            {{ text.message }}
          <a :href="text.homePage + '/commit/' + text.sha" target="_black">
            查看
          </a>
        </div>
        </div>

        <div slot="status" slot-scope="{text}">
          <a-badge :status="text.status=='Running'?'success':'default'" :text="text.status" />
        </div>
        <div slot="action" slot-scope="{text, record}">
          <a @click="log(record.name)" style="margin-right: 8px">
            <a-icon type="search"/>查看日志
          </a>
        </div>
        <template slot="statusTitle">
          <a-icon @click.native="onStatusTitleClick" type="info-circle" />
        </template>
      </standard-table>
    </a-spin>
    </div>
     <scale-container ref="scaleContainer" @refresh= "refresh" />
     <container-log ref="containerLog" @refresh= "refresh" />
  </a-card>
</template>

<script>
import StandardTable from '@/components/table/StandardTable'
import {getPodList,restartPod} from '@/services/container';
import ScaleContainer from './ScaleContainer.vue';
import ContainerLog from './ContainerLog.vue';
import { mapGetters} from 'vuex';
const columns = [
  {
    title: 'pod名称',
    dataIndex: 'name',
    key: 'name'
  },
  {
    title: '状态',
    key: 'status',
    scopedSlots: { customRender: 'status' },
  },
  {
    title: '启动时间',
    dataIndex: 'createTime',
    key: 'createTime',
  },
  {
    title: '重启次数',
    dataIndex: 'restarts',
    key: 'restarts',
  },
  {
    title: 'cpu',
    dataIndex: 'cpuUsage',
    key: 'cpuUsage',
  },
  {
    title: '内存',
    dataIndex: 'memoryUsage',
    key: 'memoryUsage',
  },
  {
    title: '操作',
    scopedSlots: { customRender: 'action' },
    width: '8%'
  }
]
export default {
  name: 'QueryList',
  components: {StandardTable,ScaleContainer,ContainerLog},
  data () {
    return {
      advanced: false,
      columns: columns,
      data: [],
      searchData:{
        pageIndex:1,
        pageSize:1000,
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
    this.pagination = {        
        pageIndex:this.searchData.pageIndex,
        pageSize:this.searchData.pageSize,
        current:1,
        showSizeChanger: true, // 显示可改变每页数量
        pageSizeOptions: ['10', '20', '50', '100'], // 每页数量选项
        showTotal: total => `Total ${total} items`, // 显示总数
        onShowSizeChange: (current, pageSize) => this.sizeChange(current,pageSize), // 改变每页数量时更新显示
        onChange:(pageIndex,pageSize)=>this.changePage(pageIndex,pageSize),//点击页码事件
        total:0 //总条数
        },
     this.getPodList(this.searchData) 
     window.setInterval(() => {
          setTimeout(() => {
            this.getPodList(this.searchData)
          }, 0)
      }, 10000 );

  },
  methods: {
    ...mapGetters('account', ['appId','envId']),
    handleSubmit(e){
      e.preventDefault();
      this.getPodList(this.searchData)
    },
    reset() {
      this.searchData = {
        pageIndex:1,
        pageSize:10
      }
    },
    refresh(){
      this.searchData.pageIndex = 1
      this.pagination.current = 1
      this.getPodList(this.searchData)
    },
    getPodList(params){
        this.spinning = true
        getPodList(params).then(this.afterGetPodList)    
    },
    afterGetPodList(res){
        this.spinning = false
        const data = res.data
        if(data.code == "0"){
            this.data = data.data
            this.pagination.total=data.total;
        }else {
            this.$message.error(data.message)
        }
    },
    restartPod(){
        this.spinning = true
        restartPod({envId:this.searchData.envId,appId:this.searchData.appId}).then(this.afterRestartPod)    
    },
    afterRestartPod(res){
        this.spinning = false
        const data = res.data
        if(data.code == "0"){
            this.refresh()
        }else {
            this.$message.error(data.message)
        }
    },
    toggleAdvanced () {
      this.advanced = !this.advanced
    },
    changePage(pageIndex,pageSize) {
         this.pagination.current = pageIndex
         this.searchData.pageIndex = pageIndex
         this.searchData.pageSize = pageSize
        this.getPodList(this.searchData)  
    },
    sizeChange(pageIndex,pageSize){
        this.searchData.pageIndex = 1
        this.searchData.pageSize = pageSize
        this.pagination.current = 1
        this.getPodList(this.searchData)  
    },
    scalePod(){
      this.$refs.scaleContainer.initData(this.searchData.envId,this.searchData.appId,this.data.length)
      this.$refs.scaleContainer.showDrawer()
    },
    log(podName){
      this.$refs.containerLog.initData(this.searchData.envId,this.searchData.appId,podName)
      this.$refs.containerLog.showDrawer()
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
