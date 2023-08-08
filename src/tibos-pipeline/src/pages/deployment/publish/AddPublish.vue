<template>
  <div>
    <a-modal v-model:visible="visible" width="1200px" title="快速部署" @ok="onSubmit">
      <a-spin :spinning="spinning">
      <standard-table
        :columns="columns"
        :dataSource="buildList"
        :pagination="pagination"
        :rowSelection=null
        :selectedRows="selectedRows"
        :selectedRowKeys="selectedRowKeys"
        rowKey="id"
        @selectedRowChange="selectedRowChange"
      >
      <div slot="tag" slot-scope="{text}">
        <a-tag :color="text.tag?'red':'blue'">
            {{text.tag?'Tag':'Branch'}}
          </a-tag>
      </div>
      <div slot="publistStatus" slot-scope="{text}">
        <a-badge :status="text.publistStatus?'processing':'default'" />
        {{ text.publistStatus?'已部署':'未部署' }}
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
      <a-form-model ref="ruleForm" :model="form" :label-col="labelCol" :wrapper-col="wrapperCol" :rules="rules">
        <a-form-model-item ref="remark" label="描述" prop="remark">
          <a-textarea
          placeholder="" :default-value="form.remark" v-model="form.remark"
          :auto-size="{ minRows: 3, maxRows: 6 }"
        />
        </a-form-model-item>
      </a-form-model>
    </a-spin>
    </a-modal>
  </div>
</template>

<script>

import StandardTable from '@/components/table/StandardTable'
import {getSuccessBuildList} from '@/services/app';
import {createPublish} from '@/services/deployment';

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
    title: '发布状态',
    key: 'publistStatus',
    scopedSlots: { customRender: 'publistStatus' }
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
    title: '用户',
    dataIndex: 'userName',
    key: 'userName',
  },
  {
    title: '描述',
    key: 'message',
    scopedSlots: { customRender: 'message' }
  }
]

export default {
  name: 'AddPublish',
  components: {StandardTable},
  data () {
    return {
      selectedRowKeys :[],
      selectedRows:[],
      columns: columns,
      actionType: 0,
      spinning: false,
      advanced: false,
      visible: false,
      labelCol: { span: 1 },
      wrapperCol: { span: 12 },
      buildList: [],
      pagination: {},
      searchData: {
        pageIndex:1,
        pageSize:10
      },
      form:{
        buildRecordId:0,
        remark:''
      },
      rules:{},
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
    }
  },
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
        }
  },
  methods: {
    initData(appId,envId){
      this.selectedRowKeys=[]
      this.selectedRows=[]
      this.form.buildRecordId = 0
      this.form.remark=''
      this.searchData.appId = appId
      this.searchData.envId = envId
      this.getSuccessBuildList(this.searchData)
    },
    showDrawer() {
      this.visible = true;
    },
    onClose() {
      this.visible = false;
    },
    getSuccessBuildList(params){
        this.spinning = true
        getSuccessBuildList(params).then(this.afterGetSuccessBuildList)    
    },
    afterGetSuccessBuildList(res){
        this.spinning = false
        const data = res.data
        if(data.code == "0"){
            this.buildList = data.data
            this.pagination.total=data.total
        }else {
            this.$message.error(data.message)
        }
    },
    onSubmit () {
      this.$refs.ruleForm.validate(async valid => {
        if (valid) {
          if(this.form.buildRecordId==''||this.form.buildRecordId==0){
            this.$message.error('请选择构建记录')
            return false;
          }
          let res = await createPublish(this.form)
          const data = res.data;
          if (data.code === '0') {
            this.spinning = false
            this.$message.success('操作成功!');
            this.$emit('refresh');
            this.onClose()

          } else {
            this.spinning = false
            this.$message.error(data.message)
          }
        } else {
          return false
        }
      })
    },
    toggleAdvanced () {
      this.advanced = !this.advanced
    },
    changePage(pageIndex,pageSize) {
         this.pagination.current = pageIndex
         this.searchData.pageIndex = pageIndex
         this.searchData.pageSize = pageSize
        this.getSuccessBuildList(this.searchData)  
    },
    sizeChange(pageIndex,pageSize){
        this.searchData.pageIndex = 1
        this.searchData.pageSize = pageSize
        this.pagination.current = 1
        this.getSuccessBuildList(this.searchData)  
    },
    selectedRowChange(selectedRowKeys, selectedRows){
        this.selectedRowKeys = selectedRowKeys
        this.selectedRows = selectedRows
        if(this.selectedRowKeys.length > 0){
          this.form.buildRecordId = this.selectedRowKeys[0]
        }
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
