import React from 'react';
import ViewFundingPage from "../pages/ViewFunding";
import {FundingPeriod, FundingStream, Specification} from "../types/viewFundingTypes";
import {FacetsEntity, PublishedProviderItems} from "../types/publishedProvider";
import {Header} from "../components/Header";

const Adapter = require('enzyme-adapter-react-16');
const enzyme = require('enzyme');
enzyme.configure({adapter: new Adapter()});
const {shallow} = enzyme;


it('shows the Beta panel', () => {
    const wrapper = shallow(<Header />);

    let actual = wrapper.find('strong.govuk-phase-banner__content__tag');

    expect(actual).toBeTruthy();
});

it('shows Calculate Funding in the service name', ()=>{
   const wrapper = shallow(<Header/>);

   let actual = wrapper.find('govuk-header__link--service-name');

   expect(actual.contains("Calculate Funding"));
});

it('shows GOV.UK in the logotype text area', ()=>{
   const wrapper = shallow(<Header/>);

   let actual = wrapper.find('govuk-header__logotype-text');

   expect(actual.contains("GOV.UK"));
});