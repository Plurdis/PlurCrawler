﻿using PlurCrawler.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlurCrawler.Search
{
    /// <summary>
    /// 국제표준 ISO 3166-1-alpha-2에 의거하는 지역 코드입니다.
    /// </summary>
    public enum RegionCode
    {
        [Note("제한 없음")]
        All,
        [Note("Afghanistan")]
        AF,
        [Note("Åland Islands")]
        AX,
        [Note("Albania")]
        AL,
        [Note("Algeria")]
        DZ,
        [Note("American Samoa")]
        AS,
        [Note("Andorra")]
        AD,
        [Note("Angola")]
        AO,
        [Note("Anguilla")]
        AI,
        [Note("Antarctica")]
        AQ,
        [Note("Antigua and Barbuda")]
        AG,
        [Note("Argentina")]
        AR,
        [Note("Armenia")]
        AM,
        [Note("Aruba")]
        AW,
        [Note("Australia")]
        AU,
        [Note("Austria")]
        AT,
        [Note("Azerbaijan")]
        AZ,
        [Note("Bahamas")]
        BS,
        [Note("Bahrain")]
        BH,
        [Note("Bangladesh")]
        BD,
        [Note("Barbados")]
        BB,
        [Note("Belarus")]
        BY,
        [Note("Belgium")]
        BE,
        [Note("Belize")]
        BZ,
        [Note("Benin")]
        BJ,
        [Note("Bermuda")]
        BM,
        [Note("Bhutan")]
        BT,
        [Note("Bolivia, Plurinational State of")]
        BO,
        [Note("Bonaire, Sint Eustatius and Saba")]
        BQ,
        [Note("Bosnia and Herzegovina")]
        BA,
        [Note("Botswana")]
        BW,
        [Note("Bouvet Island")]
        BV,
        [Note("Brazil")]
        BR,
        [Note("British Indian Ocean Territory")]
        IO,
        [Note("Brunei Darussalam")]
        BN,
        [Note("Bulgaria")]
        BG,
        [Note("Burkina Faso")]
        BF,
        [Note("Burundi")]
        BI,
        [Note("Cambodia")]
        KH,
        [Note("Cameroon")]
        CM,
        [Note("Canada")]
        CA,
        [Note("Cape Verde")]
        CV,
        [Note("Cayman Islands")]
        KY,
        [Note("Central African Republic")]
        CF,
        [Note("Chad")]
        TD,
        [Note("Chile")]
        CL,
        [Note("China")]
        CN,
        [Note("Christmas Island")]
        CX,
        [Note("Cocos(Keeling Islands")]
        CC,
        [Note("Colombia")]
        CO,
        [Note("Comoros")]
        KM,
        [Note("Congo")]
        CG,
        [Note("Congo, the Democratic Republic of the")]
        CD,
        [Note("Cook Islands")]
        CK,
        [Note("Costa Rica")]
        CR,
        [Note("Côte d'Ivoire")]
        CI,
        [Note("Croatia")]
        HR,
        [Note("Cuba")]
        CU,
        [Note("Curaçao")]
        CW,
        [Note("Cyprus")]
        CY,
        [Note("Czech Republic")]
        CZ,
        [Note("Denmark")]
        DK,
        [Note("Djibouti")]
        DJ,
        [Note("Dominica")]
        DM,
        [Note("Dominican Republic")]
        DO,
        [Note("Ecuador")]
        EC,
        [Note("Egypt")]
        EG,
        [Note("El Salvador")]
        SV,
        [Note("Equatorial Guinea")]
        GQ,
        [Note("Eritrea")]
        ER,
        [Note("Estonia")]
        EE,
        [Note("Ethiopia")]
        ET,
        [Note("Falkland Islands(Malvinas")]
        FK,
        [Note("Faroe Islands")]
        FO,
        [Note("Fiji")]
        FJ,
        [Note("Finland")]
        FI,
        [Note("France")]
        FR,
        [Note("French Guiana")]
        GF,
        [Note("French Polynesia")]
        PF,
        [Note("French Southern Territories")]
        TF,
        [Note("Gabon")]
        GA,
        [Note("Gambia")]
        GM,
        [Note("Georgia")]
        GE,
        [Note("Germany")]
        DE,
        [Note("Ghana")]
        GH,
        [Note("Gibraltar")]
        GI,
        [Note("Greece")]
        GR,
        [Note("Greenland")]
        GL,
        [Note("Grenada")]
        GD,
        [Note("Guadeloupe")]
        GP,
        [Note("Guam")]
        GU,
        [Note("Guatemala")]
        GT,
        [Note("Guernsey")]
        GG,
        [Note("Guinea")]
        GN,
        [Note("Guinea - Bissau")]
        GW,
        [Note("Guyana")]
        GY,
        [Note("Haiti")]
        HT,
        [Note("Heard Island and McDonald Islands")]
        HM,
        [Note("Holy See(Vatican City State")]
        VA,
        [Note("Honduras")]
        HN,
        [Note("Hong Kong")]
        HK,
        [Note("Hungary")]
        HU,
        [Note("Iceland")]
        IS,
        [Note("India")]
        IN,
        [Note("Indonesia")]
        ID,
        [Note("Iran, Islamic Republic of")]
        IR,
        [Note("Iraq")]
        IQ,
        [Note("Ireland")]
        IE,
        [Note("Isle of Man")]
        IM,
        [Note("Israel")]
        IL,
        [Note("Italy")]
        IT,
        [Note("Jamaica")]
        JM,
        [Note("Japan")]
        JP,
        [Note("Jersey")]
        JE,
        [Note("Jordan")]
        JO,
        [Note("Kazakhstan")]
        KZ,
        [Note("Kenya")]
        KE,
        [Note("Kiribati")]
        KI,
        [Note("Korea, Democratic People's Republic of")]
        KP,
        [Note("Korea, Republic of")]
        KR,
        [Note("Kuwait")]
        KW,
        [Note("Kyrgyzstan")]
        KG,
        [Note("Lao People's Democratic Republic")]
        LA,
        [Note("Latvia")]
        LV,
        [Note("Lebanon")]
        LB,
        [Note("Lesotho")]
        LS,
        [Note("Liberia")]
        LR,
        [Note("Libya")]
        LY,
        [Note("Liechtenstein")]
        LI,
        [Note("Lithuania")]
        LT,
        [Note("Luxembourg")]
        LU,
        [Note("Macao")]
        MO,
        [Note("Macedonia, the Former Yugoslav Republic of")]
        MK,
        [Note("Madagascar")]
        MG,
        [Note("Malawi")]
        MW,
        [Note("Malaysia")]
        MY,
        [Note("Maldives")]
        MV,
        [Note("Mali")]
        ML,
        [Note("Malta")]
        MT,
        [Note("Marshall Islands")]
        MH,
        [Note("Martinique")]
        MQ,
        [Note("Mauritania")]
        MR,
        [Note("Mauritius")]
        MU,
        [Note("Mayotte")]
        YT,
        [Note("Mexico")]
        MX,
        [Note("Micronesia, Federated States of")]
        FM,
        [Note("Moldova, Republic of")]
        MD,
        [Note("Monaco")]
        MC,
        [Note("Mongolia")]
        MN,
        [Note("Montenegro")]
        ME,
        [Note("Montserrat")]
        MS,
        [Note("Morocco")]
        MA,
        [Note("Mozambique")]
        MZ,
        [Note("Myanmar")]
        MM,
        [Note("Namibia")]
        NA,
        [Note("Nauru")]
        NR,
        [Note("Nepal")]
        NP,
        [Note("Netherlands")]
        NL,
        [Note("New Caledonia")]
        NC,
        [Note("New Zealand")]
        NZ,
        [Note("Nicaragua")]
        NI,
        [Note("Niger")]
        NE,
        [Note("Nigeria")]
        NG,
        [Note("Niue")]
        NU,
        [Note("Norfolk Island")]
        NF,
        [Note("Northern Mariana Islands")]
        MP,
        [Note("Norway")]
        NO,
        [Note("Oman")]
        OM,
        [Note("Pakistan")]
        PK,
        [Note("Palau")]
        PW,
        [Note("Palestine, State of")]
        PS,
        [Note("Panama")]
        PA,
        [Note("Papua New Guinea")]
        PG,
        [Note("Paraguay")]
        PY,
        [Note("Peru")]
        PE,
        [Note("Philippines")]
        PH,
        [Note("Pitcairn")]
        PN,
        [Note("Poland")]
        PL,
        [Note("Portugal")]
        PT,
        [Note("Puerto Rico")]
        PR,
        [Note("Qatar")]
        QA,
        [Note("R챕union")]
        RE,
        [Note("Romania")]
        RO,
        [Note("Russian Federation")]
        RU,
        [Note("Rwanda")]
        RW,
        [Note("Saint Barthelemy")]
        BL,
        [Note("Saint Helena, Ascension and Tristan da Cunha")]
        SH,
        [Note("Saint Kitts and Nevis")]
        KN,
        [Note("Saint Lucia")]
        LC,
        [Note("Saint Martin(French part")]
        MF,
        [Note("Saint Pierre and Miquelon")]
        PM,
        [Note("Saint Vincent and the Grenadines")]
        VC,
        [Note("Samoa")]
        WS,
        [Note("San Marino")]
        SM,
        [Note("Sao Tome and Principe")]
        ST,
        [Note("Saudi Arabia")]
        SA,
        [Note("Senegal")]
        SN,
        [Note("Serbia")]
        RS,
        [Note("Seychelles")]
        SC,
        [Note("Sierra Leone")]
        SL,
        [Note("Singapore")]
        SG,
        [Note("Sint Maarten(Dutch part")]
        SX,
        [Note("Slovakia")]
        SK,
        [Note("Slovenia")]
        SI,
        [Note("Solomon Islands")]
        SB,
        [Note("Somalia")]
        SO,
        [Note("South Africa")]
        ZA,
        [Note("South Georgia and the South Sandwich Islands")]
        GS,
        [Note("South Sudan")]
        SS,
        [Note("Spain")]
        ES,
        [Note("Sri Lanka")]
        LK,
        [Note("Sudan")]
        SD,
        [Note("Suriname")]
        SR,
        [Note("Svalbard and Jan Mayen")]
        SJ,
        [Note("Swaziland")]
        SZ,
        [Note("Sweden")]
        SE,
        [Note("Switzerland")]
        CH,
        [Note("Syrian Arab Republic")]
        SY,
        [Note("Taiwan, Province of China")]
        TW,
        [Note("Tajikistan")]
        TJ,
        [Note("Tanzania, United Republic of")]
        TZ,
        [Note("Thailand")]
        TH,
        [Note("Timor - Leste")]
        TL,
        [Note("Togo")]
        TG,
        [Note("Tokelau")]
        TK,
        [Note("Tonga")]
        TO,
        [Note("Trinidad and Tobago")]
        TT,
        [Note("Tunisia")]
        TN,
        [Note("Turkey")]
        TR,
        [Note("Turkmenistan")]
        TM,
        [Note("Turks and Caicos Islands")]
        TC,
        [Note("Tuvalu")]
        TV,
        [Note("Uganda")]
        UG,
        [Note("Ukraine")]
        UA,
        [Note("United Arab Emirates")]
        AE,
        [Note("United Kingdom")]
        GB,
        [Note("United States")]
        US,
        [Note("United States Minor Outlying Islands")]
        UM,
        [Note("Uruguay")]
        UY,
        [Note("Uzbekistan")]
        UZ,
        [Note("Vanuatu")]
        VU,
        [Note("Venezuela, Bolivarian Republic of")]
        VE,
        [Note("Viet Nam")]
        VN,
        [Note("Virgin Islands, British")]
        VG,
        [Note("Virgin Islands, U.S.")]
        VI,
        [Note("Wallis and Futuna")]
        WF,
        [Note("Western Sahara")]
        EH,
        [Note("Yemen")]
        YE,
        [Note("Zambia")]
        ZM,
        [Note("Zimbabwe")]
        ZW,
    }
}
