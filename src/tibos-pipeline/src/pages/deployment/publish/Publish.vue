<template>
  <a-card>
    <div>
      <a-space class="operator">
        <a-button @click="createPublish" type="primary">立即部署</a-button>
      </a-space>
    <a-spin :spinning="spinning">
      <standard-table
        :columns="columns"
        :dataSource="data"
        :pagination="pagination"
        :rowSelection=null
        rowKey="id"
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
        <a-tag :color="statusMap[text.status].color">
          {{statusMap[text.status].name}}
          </a-tag>
        </div>

        <div slot="action" slot-scope="{text, record}">
          <a @click="updatePublish(record.id)" style="margin-right: 8px">
            <a-icon type="sync"/>回滚
          </a>
          <!-- <router-link :to="`/list/query/detail/${record.key}`" >详情</router-link> -->
        </div>
        <template slot="statusTitle">
          <a-icon @click.native="onStatusTitleClick" type="info-circle" />
        </template>
      </standard-table>
    </a-spin>
    </div>
     <add-publish ref="addPublish" @refresh= "refresh" />
     <roll-back-publish ref="rollBackPublish" @refresh= "refresh" />
  </a-card>
</template>

<script>
import StandardTable from '@/components/table/StandardTable'
import {getPublishList} from '@/services/deployment';
import AddPublish from './AddPublish.vue';
import RollBackPublish from './RollBackPublish.vue';
import { mapGetters} from 'vuex';
const columns = [
  {
    title: '部署编号',
    dataIndex: 'id',
    key: 'id'
  },
  {
    title: '状态',
    key: 'status',
    scopedSlots: { customRender: 'status' },
  },
  {
    title: '应用名称',
    dataIndex: 'appName',
    key: 'appName',
  },
  {
    title: '所属项目',
    dataIndex: 'projectName',
    key: 'projectName',
  },
  {
    title: '发布人',
    dataIndex: 'nickName',
    key: 'nickName',
  },
  {
    title: '环境',
    dataIndex: 'envName',
    key: 'envName',
  },
  {
    title: '构建编号',
    dataIndex: 'buildId',
    key: 'buildId',
  },
  {
    title: '消息',
    key: 'message',
    scopedSlots: { customRender: 'message' },
  },
  {
    title: '创建时间',
    dataIndex: 'createTime',
    key: 'createTime'
  },
  {
    title: '操作',
    scopedSlots: { customRender: 'action' },
    width: '8%'
  }
]
export default {
  name: 'QueryList',
  components: {StandardTable,AddPublish,RollBackPublish},
  data () {
    return {
      advanced: false,
      columns: columns,
      data: [],
      searchData:{
        pageIndex:1,
        pageSize:10,
        envId: 0,
        appId: 0
      },
      form: this.$form.createForm(this, { name: 'search' }),
      statusMap:{'-1':{name:"部署失败",color:"red"},0:{name:"待审核",color:"orange"},1:{name:"审核通过",color:"purple"},2:{name:"部署中",color:"green"},3:{name:"部署成功",color:"blue"}},
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
     this.getPublishList(this.searchData) 
     window.setInterval(() => {
          setTimeout(() => {
            this.getPublishList(this.searchData)
          }, 0)
      }, 3000 );

  },
  methods: {
    ...mapGetters('account', ['appId','envId']),
    handleSubmit(e){
      e.preventDefault();
      this.getPublishList(this.searchData)
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
      this.getPublishList(this.searchData)
    },
    getPublishList(params){
        this.spinning = true
        getPublishList(params).then(this.afterGetPublishList)    
    },
    afterGetPublishList(res){
        this.spinning = false
        const data = res.data
        if(data.code == "0"){
            this.data = data.data
            this.pagination.total=data.total;
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
        this.getPublishList(this.searchData)  
    },
    sizeChange(pageIndex,pageSize){
        this.searchData.pageIndex = 1
        this.searchData.pageSize = pageSize
        this.pagination.current = 1
        this.getPublishList(this.searchData)  
    },
    createPublish() {
      this.$refs.addPublish.actionType = 0
      this.$refs.addPublish.initData(this.searchData.appId,this.searchData.envId)
      this.$refs.addPublish.showDrawer()
    },
    updatePublish(id){
      this.$refs.rollBackPublish.actionType = 0
      this.$refs.rollBackPublish.initData(id)
      this.$refs.rollBackPublish.showDrawer()
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
