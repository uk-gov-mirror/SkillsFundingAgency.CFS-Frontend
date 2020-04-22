import React from "react";
import OrganisationChart from "../../components/OrganisationChart";
import NodeTemplate from "../../components/TemplateBuilderNode";
import { mount } from "enzyme";
import { FundingLineOrCalculation, NodeType, FundingLineType, AggregrationType, CalculationType, FundingLineDictionaryEntry, ValueFormatType, FundingLineOrCalculationSelectedItem, FundingLine, Calculation } from "../../types/TemplateBuilderDefinitions";
import { sendDragInfo, clearDragInfo, getDragInfo, sendSelectedNodeInfo, clearSelectedNodeInfo, getSelectedNodeInfo } from "../../services/templateBuilderService";

const data: FundingLineOrCalculation = {
    id: "n1",
    isRootNode: true,
    templateLineId: 1,
    kind: NodeType.FundingLine,
    type: FundingLineType.Information,
    name: "Funding Line 1",
    fundingLineCode: "Code",
    aggregationType: AggregrationType.Average,
    children: [
        { id: "n2", isRootNode: false, templateLineId: 2, kind: NodeType.FundingLine, type: FundingLineType.Information, name: "Funding Line 2", fundingLineCode: "code", aggregationType: AggregrationType.None },
        {
            id: "n3",
            isRootNode: false,
            templateLineId: 3,
            kind: NodeType.FundingLine,
            type: FundingLineType.Payment,
            name: "Funding Line 3",
            fundingLineCode: "Code 3",
            children: [
                { id: "n4", isRootNode: false, templateCalculationId: 4, kind: NodeType.Calculation, type: CalculationType.Information, name: "Calculation 1", formulaText: "formula", valueFormat: ValueFormatType.Currency, aggregationType: AggregrationType.Sum },
                {
                    id: "n5",
                    isRootNode: false,
                    templateLineId: 5,
                    kind: NodeType.FundingLine,
                    type: FundingLineType.Information,
                    name: "Funding Line 4",
                    fundingLineCode: "code",
                    children: [
                        { id: "n6", isRootNode: false, templateCalculationId: 6, kind: NodeType.Calculation, type: CalculationType.LumpSum, name: "Calculation 2", formulaText: "formula" },
                        { id: "n7", isRootNode: false, templateCalculationId: 7, kind: NodeType.Calculation, type: CalculationType.ProviderLedFunding, name: "Calculation 3", formulaText: "formula" }
                    ]
                },
                { id: "n8", isRootNode: false, templateCalculationId: 8, kind: NodeType.Calculation, type: CalculationType.Drilldown, name: "Calculation 4", formulaText: "formula" }
            ]
        }
    ]
};

const datasource: FundingLineDictionaryEntry[] = [
    { key: 0, value: data }
];

let onClickNode: (node: FundingLineOrCalculationSelectedItem) => void;
let onClickAdd: (id: string, newChild: FundingLine | Calculation) => Promise<void>;
let changeHierarchy: (draggedItemData: FundingLineOrCalculation, draggedItemDsKey: number, dropTargetId: string, dropTargetDsKey: number) => Promise<void>;
let cloneNode: (draggedItemData: FundingLineOrCalculation, draggedItemDsKey: number, dropTargetId: string, dropTargetDsKey: number) => Promise<void>;
let openSideBar: (open: boolean) => void;

beforeEach(() => {
    onClickNode = jest.fn();
    onClickAdd = jest.fn();
    changeHierarchy = jest.fn();
    cloneNode = jest.fn();
    openSideBar = jest.fn();
    jest.clearAllMocks();
});

it("renders all nodes in datasource", () => {
    const wrapper = mount(<OrganisationChart
        datasource={datasource}
        NodeTemplate={NodeTemplate}
        onClickNode={onClickNode}
        onClickAdd={onClickAdd}
        changeHierarchy={changeHierarchy}
        cloneNode={cloneNode}
        openSideBar={openSideBar}
        editMode={true}
        nextId={9}
    />);

    expect(wrapper.find('OrganisationChartNode')).toHaveLength(8);
});

it("adds new lines", () => {
    const wrapper = mount(<OrganisationChart
        datasource={datasource}
        NodeTemplate={NodeTemplate}
        onClickNode={onClickNode}
        onClickAdd={onClickAdd}
        changeHierarchy={changeHierarchy}
        cloneNode={cloneNode}
        openSideBar={openSideBar}
        editMode={true}
        nextId={9}
    />);

    const button = wrapper.find("[data-testid='n1-add-line']");
    button.simulate('click');
    button.simulate('click');

    expect(onClickAdd).toHaveBeenCalledTimes(2);
});

it("handles drag and drop of a Funding Line (clone)", () => {
    const wrapper = mount(<OrganisationChart
        datasource={datasource}
        NodeTemplate={NodeTemplate}
        onClickNode={onClickNode}
        onClickAdd={onClickAdd}
        changeHierarchy={changeHierarchy}
        cloneNode={cloneNode}
        openSideBar={openSideBar}
        editMode={true}
        nextId={9}
    />);

    const sourceNode = wrapper.find("div#n2");
    const targetNode = wrapper.find("div#n5");
    const targetNodeCss = targetNode.getDOMNode().classList;

    sourceNode.simulate('dragStart', { dataTransfer: { setData: (format: string, data: string) => { } } });
    targetNode.simulate('drop', { ctrlKey: true, dataTransfer: { getData: () => "{ \"dsKey\": 1 }" }, currentTarget: { classList: targetNodeCss.add("allowedDrop") } });

    expect(sendDragInfo).toHaveBeenCalledTimes(1);
    expect(sendDragInfo).toHaveBeenCalledWith("n2", "FundingLine");
    expect(clearDragInfo).toBeCalledTimes(1);
    expect(cloneNode).toBeCalledTimes(1);
    expect(changeHierarchy).toBeCalledTimes(0);
});

it("handles drag and drop of a Funding Line (copy)", () => {
    const wrapper = mount(<OrganisationChart
        datasource={datasource}
        NodeTemplate={NodeTemplate}
        onClickNode={onClickNode}
        onClickAdd={onClickAdd}
        changeHierarchy={changeHierarchy}
        cloneNode={cloneNode}
        openSideBar={openSideBar}
        editMode={true}
        nextId={9}
    />);

    const sourceNode = wrapper.find("div#n2");
    const targetNode = wrapper.find("div#n5");
    const targetNodeCss = targetNode.getDOMNode().classList;

    sourceNode.simulate('dragStart', { dataTransfer: { setData: (format: string, data: string) => { } } });
    targetNode.simulate('drop', { ctrlKey: false, dataTransfer: { getData: () => "{ \"dsKey\": 1 }" }, currentTarget: { classList: targetNodeCss.add("allowedDrop") } });

    expect(sendDragInfo).toHaveBeenCalledTimes(1);
    expect(sendDragInfo).toHaveBeenCalledWith("n2", "FundingLine");
    expect(clearDragInfo).toBeCalledTimes(1);
    expect(cloneNode).toBeCalledTimes(0);
    expect(changeHierarchy).toBeCalledTimes(1);
});

it("handles drag and drop of a Calculation", () => {
    const wrapper = mount(<OrganisationChart
        datasource={datasource}
        NodeTemplate={NodeTemplate}
        onClickNode={onClickNode}
        onClickAdd={onClickAdd}
        changeHierarchy={changeHierarchy}
        cloneNode={cloneNode}
        openSideBar={openSideBar}
        editMode={true}
        nextId={9}
    />);

    const node = wrapper.find("div#n4");
    node.simulate('dragStart', { dataTransfer: { setData: (format: string, data: string) => { } } });

    expect(sendDragInfo).toHaveBeenCalledTimes(1);
    expect(sendDragInfo).toHaveBeenCalledWith("n4", "Calculation");
});