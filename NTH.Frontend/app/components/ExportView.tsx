import { stringify } from "csv-stringify/browser/esm/sync";

interface ListItem {
  id: number;
  name: string;
  gender: string;
}

const ExportView: React.FC = () => {
  const theTable: ListItem[] = [
    { id: 1, name: "hello\n", gender: "男" },
    { id: 2, name: 'moi"', gender: "男" },
    { id: 3, name: "bonjour,", gender: "女" },
    { id: 4, name: "privet", gender: "女" },
  ];
  return <div>
    <div>
      {theTable.map(x => <div key={x.id}>
        <span>{x.id}</span>
        <span>{x.name}</span>
        <span>{x.gender}</span>
      </div>)}
      <button onClick={e => {
        const csvContent = stringify(theTable);
        // with BOM for excel
        const blob = new Blob(['\uFEFF' + csvContent], { type: 'text/csv;charset=utf-8;' });
        const url = URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.setAttribute('download', '导出结果.csv');
        document.body.appendChild(link);
        link.click();
        // 清理资源
        document.body.removeChild(link);
        URL.revokeObjectURL(url);
      }}>导出</button>
    </div>
  </div>;
};

export default ExportView;
