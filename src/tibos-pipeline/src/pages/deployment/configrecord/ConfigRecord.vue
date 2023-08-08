<template>
  <a-card>
    <a-space >
        <a-button type="primary" @click="handleSubmit" >刷新容器</a-button>
    </a-space>
    <div>
    <a-spin :spinning="spinning">
      <standard-table
        :columns="columns"
        :dataSource="data"
        :pagination="pagination"
        :rowSelection=null
        rowKey="id"
      >
      <div slot="status" slot-scope="{text}">
        <a-tag :color="text.status?'blue':'red'">
            {{text.status?'挂载配置':'取消挂载'}}
          </a-tag>

      </div>
      <div slot="actionType" slot-scope="{text}">
        <a-badge :status="actionTypeMap[text.actionTypeName]" :text="text.actionTypeDesction" />
      </div>
      <div slot="content" slot-scope="{text}">
        <a-tooltip>
          <template #title>{{ text.content }}</template>
            查看配置
        </a-tooltip>
      </div>
        <template slot="statusTitle">
          <a-icon @click.native="onStatusTitleClick" type="info-circle" />
        </template>
      </standard-table>
    </a-spin>
    </div>
  </a-card>
</template>

<script>
import StandardTable from '@/components/table/StandardTable'
import {getConfigRecord} from '@/services/configmap';
import { mapGetters} from 'vuex';
const columns = [
  {
    title: '编号',
    dataIndex: 'id',
    key: 'id'
  },
  {
    title: '应用名称',
    dataIndex: 'appName',
    key: 'appName',
  },
  {
    title: '项目名称',
    dataIndex: 'projectName',
    key: 'projectName',
  },
  {
    title: '挂载路径',
    dataIndex: 'mountPath',
    key: 'mountPath',
  },
  {
    title: '子路径',
    dataIndex: 'subPath',
    key: 'subPath',
  },
  {
    title: '配置',
    key: 'content',
    scopedSlots: { customRender: 'content' }
  },
  {
    title: '挂载状态',
    key: 'status',
    scopedSlots: { customRender: 'status' }
  },
  {
    title: '说明',
    dataIndex: 'remark',
    key: 'remark',
  },
  {
    title: '操作类型',
    key: 'actionType',
    scopedSlots: { customRender: 'actionType' }
  },
  {
    title: '修改人',
    dataIndex: 'updateUserName',
    key: 'updateUserName',
  },
  {
    title: '修改时间',
    dataIndex: 'createTime',
    key: 'createTime',
  }
]
export default {
  name: 'QueryList',
  components: {StandardTable},
  data () {
    return {
      advanced: false,
      columns: columns,
      data: [],
      searchData:{
        pageIndex:1,
        pageSize:10,
        appId: '',
        envId: ''
      },
      tagMap:{
        0:{"color":"error","name":"Branch"},
        1:{"color":"processing","name":"Tag"},
      },
      actionTypeMap:{
        "Create":"default",
        "Setting":"warning",
        "EditConfig":"success",
        "EditMounts":"processing",
        "Delete":"error"
      },
      form: this.$form.createForm(this, { name: 'search' }),
      pagination: {},
      selectedRows: [],
      spinning:false
    }
  },
//   authorize: {
//     deleteRecord: 'delete'
//   },
  created:function()
  {
    this.searchData.appId =this.appId()
    this.searchData.envId =this.envId()
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
     this.getConfigRecord(this.searchData)   
  },
  methods: {
    ...mapGetters('account', ['appId','envId']),
    handleSubmit(e){
      e.preventDefault();
      this.getConfigRecord(this.searchData)
    },
    reset() {
      this.searchData = {
        projectName: '',
        appName: '',
        pageIndex:1,
        pageSize:10
      }
    },
    refresh(){
      this.searchData.pageIndex = 1
      this.pagination.current = 1
      this.getBuildList(this.searchData)
    },
    getConfigRecord(params){
        this.spinning = true
        getConfigRecord(params).then(this.afterGetConfigRecord)    
    },
    afterGetConfigRecord(res){
        this.spinning = false
        const data = res.data
        if(data.code == "0"){
            console.log(data.data)
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
        this.getConfigRecord(this.searchData)  
    },
    sizeChange(pageIndex,pageSize){
        this.searchData.pageIndex = 1
        this.searchData.pageSize = pageSize
        this.pagination.current = 1
        this.getConfigRecord(this.searchData)  
    }
  }
}
</script>

<style lang="less" scoped>
.icons-list :deep(.anticon) {
  margin-right: 6px;
  font-size: 24px;
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
</style>
