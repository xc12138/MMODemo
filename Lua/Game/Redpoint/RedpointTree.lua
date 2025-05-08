RedpointTree = RedpointTree or {}
local this = RedpointTree

this.root = nil

function RedpointTree:Init()
    this.root = RedpointNode.New("Root")
    -- 构建前缀树
    for _, name in pairs(this.NodeNames) do
        this.InsertNode(name)
    end
end

RedpointTree.NodeNames = {
    Root = "Root",

    Bag = "Root|Bag",
    Bag_stone = "Root|Bag|stone",
    Bag_stren = "Root|Bag|stren",

    -- ModelB = "Root|ModelB",
    -- ModelB_Sub_1 = "Root|ModelB|ModelB_Sub_1",
    -- ModelB_Sub_2 = "Root|ModelB|ModelB_Sub_2",
}

-- 插入
function RedpointTree.InsertNode(name)
    if IsStrNullOrEmpty(name) then
        return
    end
    if this.SearchNode(name) then
        return
    end

    local node = this.root
    node.passCnt = node.passCnt + 1
    local pathList = Split(name,"|")
    for _, value in pairs(pathList) do
        if not node.children[value] then
            node.children[value] = RedpointNode.New(value)
        end
        node = node.children[value]
        node.passCnt = node.passCnt + 1
    end
    node.endCnt = node.endCnt + 1
end

-- 查找
function RedpointTree.SearchNode(name)
    if IsStrNullOrEmpty(name) then
        return
    end
    local node = this.root
    local pathList = Split(name,"|")
    for _, path in pairs(pathList) do
        if not node.children[path] then
            return nil
        end
        node = node.children[path]
    end
    if node.endCnt > 0 then
        return node
    end

    return nil
end

-- 删除
function RedpointTree.DeleteNode(name)
    if IsStrNullOrEmpty(name) or not this.SearchNode(name) then
        return
    end
    local node = this.root
    node.passCnt = node.passCnt - 1
    local pathList = Split(name,'|')
    for _, path in pairs(pathList) do
        local childNode = node.children[path]
        childNode.passCnt = childNode.passCnt - 1
        if childNode.passCnt == 0 then
            node.children[path] = nil
            return
        end
        node = childNode
    end
    node.endCnt = node.endCnt - 1
end

-- 修改节点红点数
function RedpointTree.ChangeRedpointCnt(name, delta)
    local targetNode = this.SearchNode(name)
    if not targetNode then
        return
    end
    -- 如果是减红点,并且红点数不够减了，调整delta
    if delta < 0 and targetNode.redpointCnt + delta < 0 then
        delta = -targetNode.redpointCnt
    end

    local node = this.root
    local pathList = Split(name,'|')
    for _, path in pairs(pathList) do
        local childNode = node.children[path]
        childNode.redpointCnt = childNode.redpointCnt + delta
        node = childNode
        if next(node.updateCb) then
            -- 调用回调函数
            for _, cb in pairs(node.updateCb) do
                cb(node.redpointCnt)
            end
        end
    end
end

-- 红点更新回调函数
function RedpointTree.SetCallBack(name, key, cb)
    local node = this.SearchNode(name)
    if not node then
        return
    end
    node.updateCb[key] = cb
end

-- 查询节点红点数
function RedpointTree.GetRedpointCnt(name)
    local node = this.SearchNode(name)
    if not node then
        return 0
    end
    return node.redpointCnt or 0
end

return RedpointTree