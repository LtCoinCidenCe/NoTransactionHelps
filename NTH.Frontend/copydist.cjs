// 导入node原生模块，无需npm安装任何依赖
const fs = require('fs/promises');
const path = require('path');
const fsSync = require('fs');

// 定义源目录和目标目录（与你的bash指令路径完全对应）
const sourceDir = path.resolve(__dirname, './build/client');
const targetDir = path.resolve(__dirname, '../NTH/wwwroot');

/**
 * 递归拷贝文件/目录（等价于 cp -r -f）
 * @param {string} src 源路径
 * @param {string} dest 目标路径
 */
async function copyRecursive(src, dest) {
  try {
    // 判断源路径是否存在，不存在则直接结束
    const stat = await fs.stat(src);

    if (stat.isDirectory()) {
      // 1. 如果是【目录】：先创建目标目录 + 强制覆盖（等价于 -f）
      // recursive:true 自动创建多级目录，force:true 目录已存在不会报错
      await fs.mkdir(dest, { recursive: true, force: true });

      // 读取当前目录下所有子文件/子目录
      const entries = await fs.readdir(src, { withFileTypes: true });
      // 递归处理每一个子项
      for (const entry of entries) {
        const srcPath = path.join(src, entry.name);
        const destPath = path.join(dest, entry.name);
        await copyRecursive(srcPath, destPath);
      }
    } else {
      // 2. 如果是【文件】：拷贝文件 + 强制覆盖（等价于 -f）
      // copyFileSync 存在同名文件会直接覆盖，无任何提示，完美对应 cp -f
      fsSync.copyFileSync(src, dest);
    }
  } catch (err) {
    console.error('拷贝文件/目录失败：', err.message);
  }
}

// 执行拷贝并打印结果
(async () => {
  console.log('开始拷贝 dist 目录内容到目标路径...');
  await copyRecursive(sourceDir, targetDir);
  console.log('✅ 拷贝完成！');
})();
