//启动   node nodeTool/xlsx.js   
var xlsx = require('node-xlsx');
var fs = require('fs');
var path = require("path");
const checkIndex = 0 //用于检测数据
const typeIndex = 1//类型索引
const notationIndex = 2//注释索引
const keyIndex = 3//key索引
const dataStart = 4 //数据从第几个开始算
const tableUrl = path.resolve('./Assets/Resources/xlsx/configData\(关卡\).xlsx')//文件路径
/**
 * 生成提示文件跟json
 */
class changTool {
    constructor() {
        let result = this.parseXlsxOne(tableUrl)
        this.writeTs(result)
    }
    /**
     * 解析单张表
     * @param url 
     */
    parseXlsxOne(url) {
        const workSheetsFromFile = xlsx.parse(fs.readFileSync(url));
        let xlsxName = url.replace(/(.*\/)*([^.]+).*/ig, "$2")
        let contentNamespace = ''
        workSheetsFromFile.forEach(biaoOne => {
            let contentSheet = ''
            let biaoOneData = biaoOne.data
            let len = biaoOneData[keyIndex].length//数据的个数以key为准
            for (let i = 0; i < len; i++) {
                let notation = biaoOneData[notationIndex][i]
                let key = biaoOneData[keyIndex][i]
                contentSheet += this.templateDetailed(notation, key)
            }
            contentSheet = this.removeLineBreaks(contentSheet)
            contentNamespace += this.templateSheet(biaoOne.name, contentSheet)
        })
        contentNamespace = this.removeLineBreaks(contentNamespace)
        return contentNamespace
    }
    /**
     * 写入代码提示
     */
    writeTs(str) {
        fs.writeFileSync(path.resolve('./Assets/Scripts/Command/BiaoEnum.cs'), str) 
    }
    /**
     * 生成单个分页
     */
    templateSheet(className, content) {
        let regex1 = "\\((.+?)\\)";    // () 小括号
        let arr1 = className.match(regex1)
        let notation = arr1[1]
        let name = className.replace(arr1[0], '')
        let str =
    `/// <summary>
/// ${notation}
/// </summary>
public enum ${name}Key {
        ${content}
}
`
        return str
    }
    /**
     * 生成详情
     */
    templateDetailed(notation, keyName) {
        let str =
    `/// <summary>
        /// ${notation}
        /// </summary>
        ${keyName},
        `
        return str
    }
    /**
     * 去除换行
     * @param content 
     */
    removeLineBreaks(content) {
        return content.substr(0, content.lastIndexOf('\n'));
    }
}
/**
 * 生成json数据
 */
class loadAndCheckXlsx {
    constructor() {
        let jsonData = this.parseXlsxOne(tableUrl)
        this.writeJson(JSON.stringify(jsonData))
    }
    /**
     * 解析单张表生成json
     * @param url 
     */
    parseXlsxOne(url) {
        const workSheetsFromFile = xlsx.parse(fs.readFileSync(url));
        let xlsxFullName = url.replace(/(.*\/)*([^.]+).*/ig, "$2")
        let regex1 = "\\((.+?)\\)";    // () 小括号
        let arr1 = xlsxFullName.match(regex1)
        let xlsxName = xlsxFullName.replace(arr1[0], '')
        let json = {}
        let typeArr = []
        json[xlsxName] = {}
        workSheetsFromFile.forEach((biaoOne, biaoIndex) => {
            let regex1 = "\\((.+?)\\)";    // () 小括号
            let arr1 = biaoOne.name.match(regex1)
            let name = biaoOne.name.replace(arr1[0], '')
            json[xlsxName][name] = { '0_0key': {} }
            let biaoOneData = biaoOne.data
            let len = biaoOneData[keyIndex].length//数据的个数以key为准
            for (let i = 0; i < len; i++) {
                let type = biaoOneData[typeIndex][i]
                typeArr.push(type)
                let key = biaoOneData[keyIndex][i]
                json[xlsxName][name]['0_0key'][key] = i
            }
            for (let i = dataStart, len = biaoOneData.length; i < len; i++) {
                let hengOne = biaoOneData[i]
                hengOne = hengOne.map((data, index) => {//检测并整理数据
                    let type = typeArr[index]
                    let isErr = false
                    switch (type) {
                        case 'number':
                            isErr = typeof (data) != 'number'
                            break;
                        case 'string':
                            isErr = typeof (data) != "string"
                            break;
                        case 'boolean':
                            isErr = typeof (data) != "boolean"
                            break;
                        default:
                            try {
                                data = JSON.parse(data)
                            } catch (error) {
                                console.log(`未知类型${type}不能转Json。${xlsxFullName},第${biaoIndex + 1}页：${biaoOne.name}，第${this.changeIndexToABC(index)}列,第${i + 1}行\n`)
                            }
                            break;
                    }
                    if (isErr) {
                        console.log(`数据类型不是${type}。${xlsxFullName},第${biaoIndex + 1}页：${biaoOne.name}，第${this.changeIndexToABC(index)}列,第${i + 1}行\n`)
                    }
                    return data
                })
                json[xlsxName][name][hengOne[0]] = hengOne
            }
        })
        return json
    }
    /**
     * 生成代码
     */
    writeJson(str) {
        fs.writeFileSync(path.resolve('./Assets/StreamingAssets/configData.json'), str)
    }
    /**
     * 列索引转ABC列
     * 个位是26进制，然后是27进制????
     * 最大支持26 * 27 - 1 = 701
     */
    changeIndexToABC(index) {
        let abcStr = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ'
        let str = ''
        if (index >= 26) {
            let x = Math.floor(index / 26) - 1
            str += abcStr.substr(x, 1)
            str += abcStr.substr(index % 26, 1)
        } else {
            str = abcStr.substr(index, 1)
        }
        return str
    }
}
(function () {
    new changTool()
    new loadAndCheckXlsx()
}())