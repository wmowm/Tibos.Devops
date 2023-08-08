
//字符串取模运算
export function moduloOperation(txt,leng){
  const length = txt.length
  if(leng > length) {
    return length
  }else{
    return leng % length
  }
}