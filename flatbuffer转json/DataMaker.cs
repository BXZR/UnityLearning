using FlatBuffers;
using System.IO;
using System;
namespace Game.Config
{
public class DataMaker{

public void SaveData(string outputPath){  
var builder = new FlatBufferBuilder(1);
Offset<Data>[] offs = new Offset<Data>[8];
var item6_1 =Convert.ToUInt32(150050);
var item6_2 =builder.CreateString("关羽-普通攻击");
var item6_3 =Convert.ToUInt32(1200);
var value6 = Data.CreateData( builder , new VectorOffset() ,item6_1,item6_2,item6_3);
offs[0] = value6;
var item7_1 =Convert.ToUInt32(273000);
var item7_2 =builder.CreateString("关羽-分身斩1");
var item7_3 =Convert.ToUInt32(10000);
var value7 = Data.CreateData( builder , new VectorOffset() ,item7_1,item7_2,item7_3);
offs[1] = value7;
var item8_1 =Convert.ToUInt32(273100);
var item8_2 =builder.CreateString("关羽-拖刀斩1");
var item8_3 =Convert.ToUInt32(6000);
var value8 = Data.CreateData( builder , new VectorOffset() ,item8_1,item8_2,item8_3);
offs[2] = value8;
var item9_1 =Convert.ToUInt32(273300);
var item9_2 =builder.CreateString("关羽-偃月斩1");
var item9_3 =Convert.ToUInt32(54000);
var value9 = Data.CreateData( builder , new VectorOffset() ,item9_1,item9_2,item9_3);
offs[3] = value9;
var item10_1 =Convert.ToUInt32(283300);
var item10_2 =builder.CreateString("暗女-追日箭1");
var item10_3 =Convert.ToUInt32(60000);
var value10 = Data.CreateData( builder , new VectorOffset() ,item10_1,item10_2,item10_3);
offs[4] = value10;
var item11_1 =Convert.ToUInt32(283400);
var item11_2 =builder.CreateString("暗女-箭岚1");
var item11_3 =Convert.ToUInt32(9000);
var value11 = Data.CreateData( builder , new VectorOffset() ,item11_1,item11_2,item11_3);
offs[5] = value11;
var item12_1 =Convert.ToUInt32(283200);
var item12_2 =builder.CreateString("暗女-箭岚1(矩形收集)");
var item12_3 =Convert.ToUInt32(9000);
var value12 = Data.CreateData( builder , new VectorOffset() ,item12_1,item12_2,item12_3);
offs[6] = value12;
var item13_1 =Convert.ToUInt32(150036);
var item13_2 =builder.CreateString("暗女-普通攻击");
var item13_3 =Convert.ToUInt32(1250);
var value13 = Data.CreateData( builder , new VectorOffset() ,item13_1,item13_2,item13_3);
offs[7] = value13;

VectorOffset datas = Data.CreateDataSaveVector(builder , offs);
var saver = Data.CreateData(builder , datas ,item6_1,item6_2);
builder.Finish(saver.Value);
ByteBuffer data = builder.DataBuffer;
 if (File.Exists(outputPath)){
 File.Delete(outputPath);
}
 File.WriteAllBytes(outputPath, data.ToArray(builder.DataBuffer.Position, builder.Offset)); 
}
}
}
