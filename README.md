VertexTextureFetchForUGUI
===

Compare method to generate parameter texture.

### Texture2D.SetPixels

### Texture2D.SetPixels32

### Texture2D.LoadRawTextureData

### CommandBuffer.DrawMesh


### Demo

http://mob-sakai.github.io/VertexTextureFetchForUGUI


去年発売のAndroidで計測
```
1024x256 ARGB32
SetPixel            10.0 ms
SetPixel32           0.8 ms
LoadRawTextureData   0.3 ms
UpdateTexture(GPU)   2.0 ms

参考：コマンドバッファを利用した場合
1024x16x16 ARGB32
Mesh.set_colors32   50.0 ms <- これはひどい
CommandBuffer(GPU)   0.1 ms <- たぶん見間違い。もっと遅い
```

低解像度の場合
```
1024x16 ARGB32
SetPixel             1.0 ms
SetPixel32           0.15 ms
LoadRawTextureData   0.04 ms
UpdateTexture(GPU)   0.3 ms

参考：コマンドバッファを利用した場合
1024x16 ARGB32
Mesh.set_colors32    0.3 ms
CommandBuffer(GPU)   1.5 ms
```

コマンドバッファは1ドットあたり3頂点割当て(Triangle)
かかる時間としては LoadRawTextureData <<< SetPixel32 <<<<<<< SetPixel <<<<<<<<<<<<< CommandBuffer
