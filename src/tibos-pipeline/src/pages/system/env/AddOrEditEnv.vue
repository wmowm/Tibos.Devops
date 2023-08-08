<template>
  <div>
    <a-drawer
      :title="actionType == 0 ? `新增环境`: `编辑环境` "
      :width="480"
      :visible="visible"
      :body-style="{ paddingBottom: '80px' }"
      @close="onClose"
    >

     <a-form-model ref="ruleForm" :model="form" :label-col="labelCol" :wrapper-col="wrapperCol" :rules="rules">
        <a-form-model-item ref="name" label="环境名称" prop="name">
          <a-input
            v-model="form.name"
            @blur="
              () => {
                $refs.name.onFieldBlur();
              }
            "
          />
        </a-form-model-item>
        <a-form-model-item label="标签类型" prop='tagType'>
            <a-select  
            v-model="form.tagType"                 
            placeholder="标签类型"
            >
            <a-select-option v-for="(v,k) in this.tagTypeMap" :key="parseInt(k)">
            {{v}}
            </a-select-option>
          </a-select>
        </a-form-model-item>

        <a-form-model-item ref="key" label="标签" prop="key">
          <a-input
            v-model="form.key"
          />
        </a-form-model-item>

        <a-form-model-item ref="domainSymbol" label="域名标识" prop="domainSymbol">
          <a-input
            v-model="form.domainSymbol"
          />
        </a-form-model-item>

        <a-form-model-item ref="remark" label="描述" prop="remark">
          <a-textarea
          placeholder="" :default-value="form.remark" v-model="form.remark"
          :auto-size="{ minRows: 2, maxRows: 6 }"
        />
        </a-form-model-item>
      </a-form-model>
       <div
        :style="{
          position: 'absolute',
          right: 0,
          bottom: 0,
          width: '100%',
          borderTop: '1px solid #e9e9e9',
          padding: '10px 16px',
          background: '#fff',
          textAlign: 'right',
          zIndex: 1,
        }"
      >
        <a-button :style="{ marginRight: '8px' }" @click="onClose">
          取消
        </a-button>
        <a-button type="primary" @click="onSubmit">
          提交
        </a-button>
      </div>


    </a-drawer>
  </div>
</template>

<script>

import {getEnvById,createEnv,updateEnv} from '@/services/env';
export default {
  name: 'AddOrEditEnv',
  components: {},
  data () {
    return {
      actionType: 0,
      spinning: false,
      advanced: false,
      visible: false,
      labelCol: { span: 6 },
      wrapperCol: { span: 18 },
      tagTypeMap: {0:"Branch",1:"Tag"},
      form: {
        id: '',
        name: '',
        remark:'',
        domainSymbol:'',
        tagType:0,
        key:''
      },
      rules:{
        name: [
          { required: true, message: '请输入环境名称', trigger: 'blur' },
        ],
        tagType: [
          { required: true, message: '请选择标签类型', trigger: 'blur' },
        ]
      },
    }
  },
  created:function()
  {
      
  },
  methods: {
    initData(id){
      this.form.id = id
      if(id=='' || id==null || id== undefined){
      this.form.id=''
      this.form.name = ''
      this.form.remark = ''
      this.form.domainSymbol = ''
      this.form.tagType=''
      this.form.key=''
      }else{
        getEnvById({id:id}).then(this.afterGetEnvById)  
      }
    },
    showDrawer() {
      this.visible = true;
    },
    onClose() {
      this.visible = false;
    },
    afterGetEnvById(res){
      const data = res.data
      if(data.code == '0'){
        this.form = data.data
      }else{
            this.$message.error(data.message)
        }
    },
    onSubmit () {
      this.$refs.ruleForm.validate(async valid => {
        if (valid) {
          let res = {}
          if(this.actionType == 0){
              res = await createEnv(this.form)
          }else{
              res = await updateEnv(this.form)
          }
          const data = res.data;
          if (data.code === '0') {
            this.uploading = false
            this.$message.success('操作成功!');
            this.$emit('refresh');
            this.onClose()

          } else {
            this.uploading = false
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
