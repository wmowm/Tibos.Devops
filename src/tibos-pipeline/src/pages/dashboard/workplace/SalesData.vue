<template>
  <div style="">
    <v-chart :forceFit="true" :height="height" :data="data" :scale="scale">
      <v-tooltip :showTitle="false" dataKey="item*percent" />
      <v-axis />
      <v-legend dataKey="item" position="right" :offsetX="-140"/>
      <v-pie position="percent" color="item" :vStyle="pieStyle" :label="labelConfig" />
      <v-coord type="theta" :radius="0.75" :innerRadius="0.6" />
    </v-chart>
  </div>
</template>

<script>
const DataSet = require('@antv/data-set')

// const cpuData = [
//   { item: 'CPU一', count: 40 },
//   { item: 'CPU二', count: 21 },
//   { item: 'CPU三', count: 17 },
//   { item: 'CPU四', count: 13 },
//   { item: 'CPU五', count: 9 },
//   { item: 'CPUxx', count: 9 },
//   { item: 'CPzz', count: 9 },
//   { item: 'CPU五zz', count: 9 },
// ]


// const sourceData = [
//   { item: '内存一', count: 40 },
//   { item: '内存二', count: 21 },
//   { item: '内存三', count: 17 },
//   { item: '内存四', count: 13 },
//   { item: '内存五', count: 9 },
//   { item: '内存xx', count: 9 },
//   { item: '内存zz', count: 9 },
//   { item: '内存五zz', count: 9 },
// ]

const scale = [{
  dataKey: 'percent',
  min: 0,
  formatter: '.0%'
}]

export default {
  name: 'SalesData',
  props: {
    defaultType: String,
    cpuData: Array,
    memoryData: Array
  },
  data () {
    return {
      data:[],
      scale,
      height: 385,
      pieStyle: {
        stroke: '#fff',
        lineWidth: 1
      },
      labelConfig: ['percent', {
        formatter: (val, item) => {
          return item.point.item + ': ' + val
        }
      }]
    }
  },
  created:function(){
    const dv = new DataSet.View().source(this.cpuData)
    dv.transform({
      type: 'percent',
      field: 'count',
      dimension: 'item',
      as: 'percent'
    })
    this.data = dv.rows
  },
  watch:{
    defaultType(newVal){
      let dv = {}
      if(newVal == "cpu"){
        dv = new DataSet.View().source(this.cpuData)
      }else{
        dv = new DataSet.View().source(this.memoryData)
      }
      dv.transform({
      type: 'percent',
      field: 'count',
      dimension: 'item',
      as: 'percent'
    })
    this.data = dv.rows
    }
  }
}
</script>

<style scoped>

</style>
