import {configure} from "enzyme";
import Adapter from "enzyme-adapter-react-16";
import MutationObserver from '@sheerun/mutationobserver-shim';
import { format } from "util";
import '@testing-library/jest-dom/extend-expect';

const axios = require("axios");
const MockAdapter = require("axios-mock-adapter");
let mock = new MockAdapter(axios);

// this should throw an error on any unmocked axios call
mock.onAny().reply((x) => {
    throw new Error(`Failed to mock service call for ${x.method} ${x.url} ${x.data ? JSON.stringify(x.data) : ""} `)
});

if (window) {
    window.scrollTo = jest.fn();
    window.MutationObserver = MutationObserver;
    window.HTMLElement.prototype.scrollIntoView = function () {
    };
}

configure({adapter: new Adapter()});

const error = global.console.error;

global.console.error = function (...args) {
    const testPath = global.jasmine?.testPath ? ` ${global.jasmine.testPath} \n` : '';
    const message = format(testPath, ...args);
    error(message);
    throw new Error(message);
};