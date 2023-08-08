<template>
  <a-card>
    <div :class="advanced ? 'search' : null">
      <a-form :form="form" layout="horizontal">
        <div :class="advanced ? null: 'fold'">
        <a-row >
          <a-col :md="8" :sm="24" >
            <a-form-item 
              label="环境名称"
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
        <a-button @click="createEnv" type="primary">添加环境</a-button>
      </a-space>
    <a-spin :spinning="spinning">
      <standard-table
        :columns="columns"
        :dataSource="data"
        :pagination="pagination"
        :rowSelection=null
        rowKey="id"
      >
      <div slot="tagType" slot-scope="{text}">
        <a-badge :status="tagTypeMap[text.tagType].color" :text="tagTypeMap[text.tagType].name" />
      </div>
        <div slot="domainSymbol" slot-scope="{text}">
          <div v-if="text.domainSymbol !=null && text.domainSymbol != ''">
            <a>
              {{ `http://${text.domainSymbol}-domain.com` }}
            </a>
          </div>
          <div v-else>
            <a>
              http://domain.com
            </a>
          </div>
        </div>
        <div slot="mappingConfig" slot-scope="{text}">
          <a-switch :checked="text.mappingConfig" @change="mappingConfigChange($event,text.id)" checkedChildren="启用" unCheckedChildren="禁用" />
        </div>

        <div slot="checkPublish" slot-scope="{text}">
          <a-switch :checked="text.checkPublish" @change="checkPublishChange($event,text.id)" checkedChildren="启用" unCheckedChildren="禁用" />
        </div>

        <div slot="action" slot-scope="{text, record}">
          <a @click="updateEnv(record.id)" style="margin-right: 8px">
            <a-icon type="edit"/>编辑
          </a>
          <a-popconfirm
          v-if="data.length"
          title="确定删除?"
          @confirm="() => deleteEnv(record.id)"
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
     <add-or-edit-env ref="addOrEditEnv" @refresh= "refresh" />
  </a-card>
</template>

<script>
import StandardTable from '@/components/table/StandardTable'
import {getEnvList,deleteEnv,updateMappingConfig,updateCheckPublish} from '@/services/env';
import AddOrEditEnv from './AddOrEditEnv.vue';
const columns = [
  {
    title: '环境编号',
    dataIndex: 'id',
    key: 'id'
  },
  {
    title: '环境名称',
    dataIndex: 'name',
    key: 'name',
  },
  {
    title: '标签类型',
    key: 'tagType',
    scopedSlots: { customRender: 'tagType' }
  },
  {
    title: '关联标签',
    dataIndex: 'key',
    key: 'key',
  },
  // {
  //   title: '映射配置',
  //   key: 'mappingConfig',
  //   scopedSlots: { customRender: 'mappingConfig' }
  // },
  {
    title: '审核发布',
    key: 'checkPublish',
    scopedSlots: { customRender: 'checkPublish' }
  },
  {
    title: '生成域名',
    key: 'domainSymbol',
    scopedSlots: { customRender: 'domainSymbol' }
  },
  {
    title: '环境描述',
    dataIndex: 'remark',
    key: 'remark'
  },
  {
    title: '操作',
    scopedSlots: { customRender: 'action' },
    width: '10%'
  }
]
export default {
  name: 'QueryList',
  components: { StandardTable, AddOrEditEnv },
  data () {
    return {
      advanced: false,
      columns: columns,
      data: [],
      searchData:{
        pageIndex:1,
        pageSize:10,
        name: ''
      },
      tagTypeMap:{
        0:{"color":"error","name":"Branch"},
        1:{"color":"processing","name":"Tag"},
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
     this.getEnvList(this.searchData)   
  },
  methods: {
    mappingConfigChange(checked, id){
      console.log(`a-switch to ${checked}`, id)
      this.spinning = true
      updateMappingConfig({id:id,mappingConfig:checked}).then(this.afterUpdateMappingConfig)   
    },
    afterUpdateMappingConfig(res){
        this.spinning = false
        const data = res.data
        if(data.code == "0"){
          this.getEnvList(this.searchData)
        }else{
            this.$message.error(data.message)
        }
    },
    checkPublishChange(checked, id){
      console.log(`a-switch to ${checked}`, id)
      this.spinning = true
      updateCheckPublish({id:id,checkPublish:checked}).then(this.afterUpdateCheckPublish)   
    },
    afterUpdateCheckPublish(res){
        this.spinning = false
        const data = res.data
        if(data.code == "0"){
          this.getEnvList(this.searchData)
        }else{
            this.$message.error(data.message)
        }
    },
    handleSubmit(e){
      e.preventDefault();
      this.getEnvList(this.searchData)
    },
    reset() {
      this.searchData = {
        name: '',
        pageIndex:1,
        pageSize:10
      }
    },
    refresh(){
      this.searchData.pageIndex = 1
      this.pagination.current = 1
      this.getEnvList(this.searchData)
    },
    getEnvList(params){
        this.spinning = true
        getEnvList(params).then(this.afterGetEnvList)    
    },
    afterGetEnvList(res){
        this.spinning = false
        const data = res.data
        if(data.code == "0"){
            console.log(data.data)
            this.data = data.data
            this.pagination.total=data.total;
        }else{
            this.$message.error(data.message)
        }
    },
    deleteEnv(id) {
      this.spinning = true
      deleteEnv({id:id}).then(this.afterDeleteEnv)    
     
    },
    afterDeleteEnv(res){
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
        this.getEnvList(this.searchData)  
    },
    sizeChange(pageIndex,pageSize){
        this.searchData.pageIndex = 1
        this.searchData.pageSize = pageSize
        this.pagination.current = 1
        this.getEnvList(this.searchData)  
    },
    createEnv() {
      this.$refs.addOrEditEnv.actionType = 0
      this.$refs.addOrEditEnv.initData('')
      this.$refs.addOrEditEnv.showDrawer()
    },
    updateEnv (id) {
      this.$refs.addOrEditEnv.actionType = 1
      this.$refs.addOrEditEnv.initData(id)
      this.$refs.addOrEditEnv.showDrawer()
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
