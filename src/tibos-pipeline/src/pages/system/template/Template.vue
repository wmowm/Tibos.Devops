<template>
  <a-card>
    <div :class="advanced ? 'search' : null">
      <a-form :form="form" layout="horizontal">
        <div :class="advanced ? null: 'fold'">
        <a-row >
          <a-col :md="8" :sm="24" >
            <a-form-item 
              label="模板名称"
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
        <a-button @click="createTemplate" type="primary">添加模板</a-button>
      </a-space>
    <a-spin :spinning="spinning">
      <standard-table
        :columns="columns"
        :dataSource="data"
        :pagination="pagination"
        :rowSelection=null
        rowKey="id"
      >
        <div slot="action" slot-scope="{text, record}">
          <a @click="download(record.id)" style="margin-right: 8px">
            <a-icon type="download"/>下载
          </a>
          <a @click="updateTemplate(record.id)" style="margin-right: 8px">
            <a-icon type="edit"/>编辑
          </a>
          <a-popconfirm
          v-if="data.length"
          title="确定删除?"
          @confirm="() => deleteTemplate(record.id)"
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
     <add-or-edit-template ref="addOrEditTemplate" @refresh= "refresh" />
  </a-card>
</template>

<script>
import StandardTable from '@/components/table/StandardTable'
import {getTemplateList,deleteTemplate,download} from '@/services/template';
import AddOrEditTemplate from './AddOrEditTemplate.vue';
const columns = [
  {
    title: '模板编号',
    dataIndex: 'id',
    key: 'id'
  },
  {
    title: '模板名称',
    dataIndex: 'name',
    key: 'name',
  },
  {
    title: '占位变量',
    dataIndex: 'tempVal',
    key: 'tempVal',
  },
  {
    title: '模板描述',
    dataIndex: 'remark',
    key: 'remark'
  },
  {
    title: '路径',
    dataIndex: 'path',
    key: 'path'
  },
  {
    title: '创建人',
    dataIndex: 'createUserName',
    key: 'createUserName'
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
  components: {StandardTable,AddOrEditTemplate},
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
     this.getTemplateList(this.searchData)   
  },
  methods: {
    handleSubmit(e){
      e.preventDefault();
      this.getTemplateList(this.searchData)
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
      this.getTemplateList(this.searchData)
    },
    download(id){
      this.spinning = true
      download({id:id})
      this.spinning = false
    },
    getTemplateList(params){
        this.spinning = true
        getTemplateList(params).then(this.afterGetTemplateList)    
    },
    afterGetTemplateList(res){
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
    deleteTemplate(id) {
      this.spinning = true
      deleteTemplate({id:id}).then(this.afterDeleteTemplate)    
     
    },
    afterDeleteTemplate(res){
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
        this.getTemplateList(this.searchData)  
    },
    sizeChange(pageIndex,pageSize){
        this.searchData.pageIndex = 1
        this.searchData.pageSize = pageSize
        this.pagination.current = 1
        this.getTemplateList(this.searchData)  
    },
    createTemplate() {
      this.$refs.addOrEditTemplate.actionType = 0
      this.$refs.addOrEditTemplate.initData('')
      this.$refs.addOrEditTemplate.showDrawer()
    },
    updateTemplate (id) {
      this.$refs.addOrEditTemplate.actionType = 1
      this.$refs.addOrEditTemplate.initData(id)
      this.$refs.addOrEditTemplate.showDrawer()
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
