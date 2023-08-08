<template>
  <div>
    <a-modal v-model:visible="visible" @ok="handleOk"  width="600px" title="应用信息">
      <a-spin :spinning="spinning">
        <a-row class="rowt">
          <a-col :span="8" class="colt coltbk">应用名称</a-col>
          <a-col :span="16" class="colt">{{ data.name }}</a-col>
        </a-row>
        <a-row class="rowt">
          <a-col :span="8" class="colt coltbk">所属项目</a-col>
          <a-col :span="16" class="colt">{{ data.projectName }}</a-col>
        </a-row>
        <a-row class="rowt">
          <a-col :span="8" class="colt coltbk">模板名称</a-col>
          <a-col :span="16" class="colt">{{ data.templateName }}</a-col>
        </a-row>
        <a-row class="rowt">
          <a-col :span="8" class="colt coltbk">组</a-col>
          <a-col :span="16" class="colt">{{ data.group }}</a-col>
        </a-row>

        <a-row class="rowt">
          <a-col :span="8" class="colt coltbk">应用地址</a-col>
          <a-col :span="16" class="colt"><a>{{ data.webUrl }}</a></a-col>
        </a-row>
        <a-row class="rowt" v-for="(v,k) in data.domainMap" :key="k">
          <a-col :span="8" class="colt coltbk" :style="{lineHeight: (v.length==0?1:v.length)*42 +'px'}">
              {{ k }}
          </a-col>
          <a-col :span="16" class="colt" >
              <a v-for="(item,index) in v" :key="index">
                {{ item}} <br />
              </a>
          </a-col>
        </a-row>
        <a-row class="rowt">
          <a-col :span="8" class="colt coltbk">描述</a-col>
          <a-col :span="16" class="colt">{{ data.remark }}</a-col>
        </a-row>

        <a-row class="rowt">
          <a-col :span="8" class="colt coltbk">创建人</a-col>
          <a-col :span="16" class="colt">{{ data.createUserName }}</a-col>
        </a-row>
        <a-row class="rowt rowtbottom"> 
          <a-col :span="8" class="colt coltbk">创建时间</a-col>
          <a-col :span="16" class="colt">{{ data.createTime }}</a-col>
        </a-row>
      </a-spin>
    </a-modal>
  </div>
</template>

<script>

import {getAppById} from '@/services/app';
export default {
  name: 'ShowApp',
  data () {
    return {

      spinning: false,
      advanced: false,
      visible: false,
      labelCol: { span: 4 },
      wrapperCol: { span: 20 },
      form:{
        id:0,
      },
      data:{},
    }
  },
  created:function()
  {
    
  },
  methods: {
    initData(id){
      this.form.id =id
      this.getAppById()
    },
    getAppById(){
      getAppById(this.form).then(this.afterGetAppById)  
    },
    afterGetAppById(res){
      const data = res.data
      if(data.code == '0'){
        this.data = data.data
      }
    },
    showDrawer() {
      this.visible = true;
    },
    onClose() {
      this.visible = false;
    },
    handleOk(){
      this.visible = false;
    },
    toggleAdvanced () {
      this.advanced = !this.advanced
    },
  }
}
</script>

<style lang="less" scoped>

.rowt{
    border-right: 1px solid rgb(232,232,232);
  }
.rowtbottom{
  border-bottom: 1px solid rgb(232,232,232);
}
  .colt{
    min-height: 42px;
    line-height: 42px;
    text-align: center;
    border-left: 1px solid rgb(232,232,232);
    border-top: 1px solid rgb(232,232,232);
    padding-left: 12px;
  }
  .coltbk{
    background-color: rgb(240,248,255);
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
