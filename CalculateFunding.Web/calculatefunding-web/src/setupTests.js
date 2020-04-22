import { configure } from "enzyme";
import Adapter from "enzyme-adapter-react-16";
import 'jest-canvas-mock';

configure({ adapter: new Adapter() });

jest.mock('./services/templateBuilderService', () => {
    const { Subject } = require('rxjs');
    const subject1 = new Subject();
    const subject2 = new Subject();
    return {
        sendDragInfo: jest.fn((id, kind) => subject1.next({ draggedNodeId: id, draggedNodeKind: kind })),
        clearDragInfo: jest.fn(() => subject1.next()),
        getDragInfo: jest.fn(() => subject1.asObservable()),
        sendSelectedNodeInfo: jest.fn(id => subject2.next({ selectedNodeId: id })),
        clearSelectedNodeInfo: jest.fn(() => subject2.next()),
        getSelectedNodeInfo: jest.fn(() => subject2.asObservable())
    }
});