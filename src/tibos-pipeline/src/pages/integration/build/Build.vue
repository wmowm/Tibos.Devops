<template>
  <a-card>
    <div :class="advanced ? 'search' : null">
      <a-form :form="form" layout="horizontal">
        <div :class="advanced ? null: 'fold'">
        <a-row >
          <a-col :md="8" :sm="24" >
            <a-form-item 
              label="项目名称"
              :labelCol="{span: 5}"
              :wrapperCol="{span: 18, offset: 1}"
            >
             <a-input v-model="searchData.projectName" />
            </a-form-item>
          </a-col>
          <a-col :md="8" :sm="24" >
            <a-form-item 
              label="应用名称"
              :labelCol="{span: 5}"
              :wrapperCol="{span: 18, offset: 1}"
            >
             <a-input v-model="searchData.appName" />
            </a-form-item>
          </a-col>
        </a-row>
        </div>
        <span style="float: right; margin-top: 3px;">
          <a-button @click="handleSubmit" type="primary">查询</a-button>
          <a-button @click="reset" style="margin-left: 8px">重置</a-button>
          <a @click="toggleAdvanced" style="margin-left: 8px">
            {{advanced ? '收起' : '展开'}}
            <a-icon :type="advanced ? 'up' : 'down'" />
          </a>
        </span>
      </a-form>
    </div>
    <div>
    <a-spin :spinning="spinning">
      <standard-table
        :columns="columns"
        :dataSource="data"
        :pagination="pagination"
        :rowSelection=null
        rowKey="id"
      >
      <div slot="tag" slot-scope="{text}">
        <a-tag :color="text.tag?'red':'blue'">
            {{text.tag?'Tag':'Branch'}}
          </a-tag>

      </div>
      <div slot="buildStatus" slot-scope="{text}">
        <a-badge :status="statusMap[text.buildStatus]" :text="text.buildStatus" />
      </div>
        <div slot="message" slot-scope="{text}">
          <div v-if="text.message !=null && text.message != ''">
            {{ text.message }}
          <a :href="text.homePage + '/commit/' + text.sha" target="_black">
            查看
          </a>
        </div>
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
import {getBuildList} from '@/services/app';

const columns = [
  {
    title: '构建编号',
    dataIndex: 'buildId',
    key: 'buildId'
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
    title: '所属团队',
    dataIndex: 'teamName',
    key: 'teamName',
  },
  {
    title: '构建状态',
    key: 'buildStatus',
    scopedSlots: { customRender: 'buildStatus' }
  },
  {
    title: '环境',
    dataIndex: 'envName',
    key: 'envName',
  },
  {
    title: '分支',
    dataIndex: 'ref',
    key: 'ref',
  },
  {
    title: '标签类型',
    key: 'tag',
    scopedSlots: { customRender: 'tag' }
  },
  {
    title: '用户',
    dataIndex: 'userName',
    key: 'userName',
  },
  {
    title: '构建时长',
    dataIndex: 'buildDuration',
    key: 'buildDuration',
  },
  {
    title: '构建时间',
    dataIndex: 'buildCreateTime',
    key: 'buildCreateTime',
  },
  {
    title: '描述',
    key: 'message',
    scopedSlots: { customRender: 'message' }
  },
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
        projectName: '',
        appName: ''
      },
      tagMap:{
        0:{"color":"error","name":"Branch"},
        1:{"color":"processing","name":"Tag"},
      },
      statusMap:{
        "created":"default",
        "pending":"warning",
        "running":"success",
        "success":"processing",
        "failed":"error"
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
     this.getBuildList(this.searchData)   
  },
  methods: {
    handleSubmit(e){
      e.preventDefault();
      this.getBuildList(this.searchData)
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
    getBuildList(params){
        this.spinning = true
        getBuildList(params).then(this.afterGetBuildList)    
    },
    afterGetBuildList(res){
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
        this.getBuildList(this.searchData)  
    },
    sizeChange(pageIndex,pageSize){
        this.searchData.pageIndex = 1
        this.searchData.pageSize = pageSize
        this.pagination.current = 1
        this.getBuildList(this.searchData)  
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
