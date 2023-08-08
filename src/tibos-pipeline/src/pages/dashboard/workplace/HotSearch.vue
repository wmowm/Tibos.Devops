<template>
  <div class="hot-search">
    <a-row style="margin: 0 -34px">
      <a-col style="padding: 0 34px; margin-bottom: 24px" :sm="12" :xs="24">
        <div class="num-info">
          <span class="title">
            CPU
            <a-tooltip title="指标说明">
              <a-icon type="info-circle" style="font-size: 14px; margin-left: 8px" />
            </a-tooltip>
          </span>
          <div class="value">
            <span class="total">{{ cpuData.length>0?cpuData[0].X:0 }}</span>
          </div>
        </div>
        <mini-area :data="cpuData" style="height: 45px" />
      </a-col>
      <a-col style="padding: 0 34px; margin-bottom: 24px" :sm="12" :xs="24">
        <div class="num-info">
          <span class="title">
            内存
            <a-tooltip title="指标说明">
              <a-icon type="info-circle" style="font-size: 14px; margin-left: 8px" />
            </a-tooltip>
          </span>
          <div class="value">
            <span class="total">{{ memoryData.length>0?memoryData[0].X:0 }}</span>
          </div>
        </div>
        <mini-area :data="memoryData" style="height: 45px" />
      </a-col>
    </a-row>
    <a-table
      :dataSource="data"
      :columns="columns"
      :pagination="{style: { marginBottom: 0 }, pageSize: 5}"
      size="small"
      rowKey="id"
    >
    
    </a-table>
  </div>
</template>

<script>
import MiniArea from '../../../components/chart/MiniArea'


const columns = [
  {
    title: '编号',
    dataIndex: 'id',
    key: 'id'
  },
  {
    title:'应用名称',
    dataIndex: 'name',
    key: 'name',
    scopedSlots: {customRender: 'name'}
  },
  {
    title: 'pod数量',
    dataIndex: 'podCount',
    key: 'podCount'
  },
  {
    title: 'cpu使用量',
    dataIndex: 'cpuUsage',
    key: 'cpuUsage',
    sorter: (a, b) => a.cpuUsage - b.cpuUsage
  },
  {
    title: '内存使用量',
    dataIndex: 'memoryUsage',
    key: 'memoryUsage',
    sorter: (a, b) => a.memoryUsage - b.memoryUsage
  },
]


export default {
  name: 'HotSearch',
  props: {
    data: Array,
    cpuData:Array,
    memoryData:Array
  },
  components: {MiniArea},
  data () {
    return {
      columns:columns,
    }
  },
}
</script>

<style lang="less" scoped>
  .num-info{
    .title{
      color: @text-color-second;
      font-size: 14px;
      height: 22px;
      line-height: 22px;
      overflow: hidden;
      text-overflow: ellipsis;
      word-break: break-all;
      white-space: nowrap;
    }
    .value{
      .total{
        color: @title-color;
        display: inline-block;
        line-height: 32px;
        height: 32px;
        font-size: 24px;
        margin-right: 32px;
      }
      .subtotal{
        color: @text-color-second;
        font-size: 16px;
        vertical-align: top;
        margin-right: 0;
        i{
          font-size: 12px;
          color: red;
          transform: scale(.82);
          margin-left: 4px;
        }
      }
    }
  }
</style>
