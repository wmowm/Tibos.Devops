<template>
  <a-card>
    <div :class="advanced ? 'search' : null">
      <a-form :form="form" layout="horizontal">
        <div :class="advanced ? null: 'fold'">
        <a-row >
          <a-col :md="8" :sm="24" >
            <a-form-item 
              label="项目编号"
              :labelCol="{span: 5}"
              :wrapperCol="{span: 18, offset: 1}"
            >
             <a-input v-model="searchData.id" />
            </a-form-item>
          </a-col>
          <a-col :md="8" :sm="24" >
            <a-form-item 
              label="项目名称"
              :labelCol="{span: 5}"
              :wrapperCol="{span: 18, offset: 1}"
            >
             <a-input v-model="searchData.name" />
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
      <a-space class="operator">
        <a-button @click="createProject" type="primary">添加项目</a-button>
      </a-space>
    <a-spin :spinning="spinning">
      <standard-table
        :columns="columns"
        :dataSource="data"
        :pagination="pagination"
        :rowSelection=null
        rowKey="id"
      >
        <div slot="domain" slot-scope="{text}">
          <div v-if="text.domain !=null && text.domain != ''">
          <a>
            {{ text.domain }}
          </a>
        </div>
        </div>
        <div slot="action" slot-scope="{text, record}">
          <router-link style="margin-right: 8px" :to="`/integration/app?projectId=${record.id}`" >
          <a-icon type="plus"/>添加应用
        </router-link>
          <a @click="updateProject(record.id)" style="margin-right: 8px">
            <a-icon type="edit"/>编辑
          </a>
          <a-popconfirm
          v-if="data.length"
          title="确定删除?"
          @confirm="() => deleteProject(record.id)"
        >
          <a href="javascript:;"><a-icon type="delete" />删除</a>
        </a-popconfirm>

          <!-- <router-link :to="`/list/query/detail/${record.key}`" >详情</router-link> -->
        </div>
        <template slot="statusTitle">
          <a-icon @click.native="onStatusTitleClick" type="info-circle" />
        </template>
      </standard-table>
    </a-spin>
    </div>
     <add-or-edit-project ref="addOrEditProject" @refresh= "refresh" />
  </a-card>
</template>

<script>
import StandardTable from '@/components/table/StandardTable'
import {getProjectList,deleteProject} from '@/services/project';
import AddOrEditProject from './AddOrEditProject.vue';
const columns = [
  {
    title: '项目编号',
    dataIndex: 'id',
    key: 'id'
  },
  {
    title: '项目名称',
    dataIndex: 'name',
    key: 'name',
  },
  {
    title: '所属团队',
    dataIndex: 'teamName',
    key: 'teamName',
  },
  {
    title: '域名',
    key: 'domain',
    scopedSlots: { customRender: 'domain' }
  },
  {
    title: '项目描述',
    dataIndex: 'remark',
    key: 'remark'
  },
  {
    title: '应用数量',
    dataIndex: 'appCount',
    key: 'appCount'
  },
  {
    title: '创建时间',
    dataIndex: 'createTime',
    key: 'createTime'
  },
  {
    title: '操作',
    scopedSlots: { customRender: 'action' },
    width: '25%'
  }
]
export default {
  name: 'QueryList',
  components: {StandardTable,AddOrEditProject},
  data () {
    return {
      advanced: false,
      columns: columns,
      data: [],
      searchData:{
        pageIndex:1,
        pageSize:10,
        id: '',
        name: ''
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
     this.getProjectList(this.searchData)   
  },
  methods: {
    handleSubmit(e){
      e.preventDefault();
      this.getProjectList(this.searchData)
    },
    reset() {
      this.searchData = {
        id: '',
        name: '',
        remark: '',
        pageIndex:1,
        pageSize:10
      }
    },
    refresh(){
      this.searchData.pageIndex = 1
      this.pagination.current = 1
      this.getProjectList(this.searchData)
    },
    getProjectList(params){
        this.spinning = true
        getProjectList(params).then(this.afterGetProjectList)    
    },
    afterGetProjectList(res){
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
    deleteProject(id) {
      this.spinning = true
      deleteProject({id:id}).then(this.afterDeleteProject)    
     
    },
    afterDeleteProject(res){
      this.spinning = false
      const data = res.data;
      if (data.code == '0') {
            this.$message.success('操作成功!');
            this.refresh();
          } else {
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
        this.getProjectList(this.searchData)  
    },
    sizeChange(pageIndex,pageSize){
        this.searchData.pageIndex = 1
        this.searchData.pageSize = pageSize
        this.pagination.current = 1
        this.getProjectList(this.searchData)  
    },
    createProject() {
      this.$refs.addOrEditProject.actionType = 0
      this.$refs.addOrEditProject.initData('')
      this.$refs.addOrEditProject.showDrawer()
    },
    updateProject (id) {
      this.$refs.addOrEditProject.actionType = 1
      this.$refs.addOrEditProject.initData(id)
      this.$refs.addOrEditProject.showDrawer()
    }
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
