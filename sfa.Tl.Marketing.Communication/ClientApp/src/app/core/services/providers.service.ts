import { Injectable } from '@angular/core';
import { IProvider } from '@core/models/provider.model';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable()
export class ProvidersService {
    public PROVIDERS;

    constructor(private http: HttpClient) { }

    getProviders(subject: string = 'all', distance: number, centerPoint: any = undefined): IProvider[] {
        let result: IProvider[];

        if (!subject || subject === 'all') {
            result = PROVIDERS;
        } else {
          result = PROVIDERS.filter(p => {
              return p.subjects.includes(subject);
          });
        }

        if (!distance || !centerPoint) {
          for (const provider of result) {
              provider.isWithinDistance = true;
          }
        } else {
          for (const provider of result) {
              provider.isWithinDistance = this.isMarkWithinRadius(centerPoint,
                  { lat: provider.location.latitude, lng: provider.location.longitude }, distance);
          }
        }

        return result;
    }

    isMarkWithinRadius(centerPoint, checkPoint, km) {
        const ky = 40000 / 360;
        const kx = Math.cos(Math.PI * centerPoint.lat / 180.0) * ky;
        const dx = Math.abs(centerPoint.lng - checkPoint.lng) * kx;
        const dy = Math.abs(centerPoint.lat - checkPoint.lat) * ky;

        return Math.sqrt(dx * dx + dy * dy) <= km;
    }

    getGeocodeJson(postcode: string): Observable<any> {
        const params = new HttpParams()
            .set("address", `${postcode}, UK`)
            .set("key", environment.apiKey);

        return this.http.get<any>(environment.apiPostcodeConvertEndpoint, { params });
    }
}

const PROVIDERS: IProvider[] = [
    {
        id: 1,
        name: "Access Creative College (Access to Music Ltd.)",
        location: {
            fullAddress: "68 Heath Mill Lane, Digbeth, Birmingham, B9 4AR",
            postcode: "B9 4AR",
            latitude: 52.4769665,
            longitude: -1.882333
        },
        phoneNumber: "03301 233155",
        website: "https://www.accesscreative.ac.uk/locations/birmingham-college/",
        subjects: ["digital"]
    },
    {
        id: 2,
        name: "Barnsley College",
        location: {
            fullAddress: "Old Mill Lane, Barnsley, S70 2LA",
            postcode: "S70 2LA",
            latitude: 53.5577154,
            longitude: -1.4824051
        },
        phoneNumber: "01226 216123",
        website: "https://www.barnsley.ac.uk/",
        subjects: ["digital", "construction", "educationChildcare"]
    },
    {
        id: 3,
        name: "Bedfordshire & Luton Education Business Partnership",
        location: {
            fullAddress: "Trust House, Unit 5, Trust Industrial Estate, Wilbury Way, Hitchin, Herts, SG4 0UZ",
            postcode: "SG4 0UZ",
            latitude: 51.963504,
            longitude: -0.266138
        },
        phoneNumber: "01462 510117",
        website: "http://www.developebp.co.uk/locations/hitchin/",
        subjects: ["digital"]
    },
    {
        id: 4,
        name: "Bishop Burton College",
        location: {
            fullAddress: "York Road, Bishop Burton, HU17 8QG",
            postcode: "HU17 8QG",
            latitude: 53.846419,
            longitude: -0.509006
        },
        phoneNumber: "01964 553000",
        website: "https://www.bishopburton.ac.uk",
        subjects: ["educationChildcare"]
    },
    {
        id: 5,
        name: "Blackpool and The Fylde College",
        location: {
            fullAddress: "Park Road, Blackpool, FY1 4ES",
            postcode: "FY1 4ES",
            latitude: 53.812887,
            longitude: -3.042857
        },
        phoneNumber: "012533 52352",
        website: "https://www.blackpool.ac.uk/",
        subjects: ["digital", "construction", "educationChildcare"]
    },
    {
        id: 6,
        name: "Bridgwater & Taunton College",
        location: {
            fullAddress: "Bath Road, Bridgwater, Somerset, TA6 4PZ",
            postcode: "TA6 4PZ",
            latitude: 51.131582,
            longitude: -2.98756
        },
        phoneNumber: "01278 455464",
        website: "https://www.btc.ac.uk/",
        subjects: ["digital", "construction", "educationChildcare"]
    },
    {
        id: 7,
        name: "Cardinal Newman College",
        location: {
            fullAddress: "Lark Hill Road, Preston, Lancashire, PR1 4HD",
            postcode: "PR1 4HD",
            latitude: 53.756497,
            longitude: -2.688952
        },
        phoneNumber: "01772 460181",
        website: "https://www.cardinalnewman.ac.uk/",
        subjects: ["digital", "educationChildcare"]
    },
    {
        id: 8,
        name: "Chichester College Group",
        location: {
            fullAddress: "Westgate Fields, Chichester, West Sussex, PO19 1SB",
            postcode: "PO19 1SB",
            latitude: 50.834834,
            longitude: -0.790433
        },
        phoneNumber: "01243 786 321",
        website: "https://chichestercollegegroup.ac.uk/",
        subjects: ["digital", "construction", "educationChildcare"]
    },
    {
        id: 9,
        name: "Cirencester College",
        location: {
            fullAddress: "Stroud Road, Cirencester, Gloucestershire, GL7 1XA",
            postcode: "GL7 1XA",
            latitude: 51.713138,
            longitude: -1.986228
        },
        phoneNumber: "01285 640994",
        website: "https://www.cirencester.ac.uk",
        subjects: ["digital", "construction", "educationChildcare"]
    },
    {
        id: 10,
        name: "City College Norwich",
        location: {
            fullAddress: "City College Norwich, Ipswich Road, Norwich, NR2 2LJ",
            postcode: "NR2 2LJ",
            latitude:  52.618145,
            longitude: 1.286295
        },
        phoneNumber: "01603 773311",
        website: "https://www.ccn.ac.uk/",
        subjects: ["digital", "construction", "educationChildcare"]
    },
    {
        id: 11,
        name: "City of Stoke-on-Trent Sixth Form College",
        location: {
            fullAddress: "Leek Road, Stoke, Staffordshire, ST4 2RU",
            postcode: "ST4 2RU",
            latitude: 53.006841,
            longitude: -2.178748
        },
        phoneNumber: "01782 848736",
        website: "https://www.stokesfc.ac.uk",
        subjects: ["digital"]
    },
    {
        id: 12,
        name: "Cranford Community College",
        location: {
            fullAddress: "High Street, Cranford, Hounslow, Middlesex, TW5 9PD",
            postcode: "TW5 9PD",
            latitude: 51.487365,
            longitude: -0.408208
        },
        phoneNumber: "020 8897 2001",
        website: "www.cranford.hounslow.sch.uk",
        subjects: ["digital", "educationChildcare"]
    },
    {
        id: 13,
        name: "Derby College",
        location: {
            fullAddress: "The Joseph Wright Centre, Cathedral Rd, Derby, DE1 3PA",
            postcode: "DE1 3PA",
            latitude: 52.925453,
            longitude: -1.479668
        },
        phoneNumber: "01332 387473",
        website: "https://www.derby-college.ac.uk/",
        subjects: ["digital", "construction", "educationChildcare"]
    },
    {
        id: 14,
        name: "Dudley College of Technology",
        location: {
            fullAddress: "The Broadway, Dudley, West Midlands, DY1 4AS",
            postcode: "DY1 4AS",
            latitude: 52.515265,
            longitude: -2.081658
        },
        phoneNumber: "01384 363 000",
        website: "https://www.dudleycol.ac.uk/",
        subjects: ["digital", "construction", "educationChildcare"]
    },
    {
        id: 15,
        name: "Durham Sixth Form Centre",
        location: {
            fullAddress: "Providence Row, The Sands, Durham City, DH1 1SG",
            postcode: "DH1 1SG",
            latitude: 54.780295,
            longitude: -1.573583
        },
        phoneNumber: "0191 383 0708",
        website: "www.durhamsixthformcentre.org.uk/",
        subjects: ["digital"]
    },
    {
        id: 16,
        name: "East Sussex College Group",
        location: {
            fullAddress: "Cross Levels Way, Eastbourne, East Sussex, BN21 2UF",
            postcode: "BN21 2UF",
            latitude: 50.78844,
            longitude: 0.272597
        },
        phoneNumber: "030 300 39699",
        website: "www.escg.ac.uk",
        subjects: ["digital", "construction", "educationChildcare"]
    },
    {
        id: 17,
        name: "Exeter College",
        location: {
            fullAddress: "Hele Road, Exeter, EX4 4JS",
            postcode: "EX4 4JS",
            latitude: 50.728216,
            longitude: -3.538138
        },
        phoneNumber: "01392 400500",
        website: "https://www.exe-coll.ac.uk/",
        subjects: ["digital", "construction", "educationChildcare"]
    },
    {
        id: 18,
        name: "Fareham College",
        location: {
            fullAddress: "Bishopsfield Road, Fareham, Hampshire, PO14 1NH",
            postcode: "PO14 1NH",
            latitude: 50.851515,
            longitude: -1.199578
        },
        phoneNumber: "01329 815 200",
        website: "https://www.fareham.ac.uk/",
        subjects: ["digital", "educationChildcare"]
    },
    {
        id: 19,
        name: "Farnborough College of Technology",
        location: {
            fullAddress: "Boundary Road, Farnborough, Hants, GU14 6SB",
            postcode: "GU14 6SB",
            latitude: 51.285753,
            longitude: -0.750885
        },
        phoneNumber: "01252 407040",
        website: "https://www.farn-ct.ac.uk/",
        subjects: ["digital", "educationChildcare"]
    },
    {
        id: 20,
        name: "Gateshead College",
        location: {
            fullAddress: "Quarryfield Road, Baltic Business Quarter, Gateshead, NE8 3BE",
            postcode: "NE8 3BE",
            latitude: 54.966187,
            longitude: -1.598803
        },
        phoneNumber: "0191 490 0300",
        website: "www.gateshead.ac.uk/",
        subjects: ["digital", "educationChildcare"]
    },
    {
        id: 21,
        name: "Grimsby Institute of Further & Higher Education",
        location: {
            fullAddress: "Nuns Corner, Grimsby, North East Lincolnshire, DN34 5BQ",
            postcode: "DN34 5BQ",
            latitude: 53.553721,
            longitude: -0.092816
        },
        phoneNumber: "0800 315 002",
        website: "https://grimsby.ac.uk/",
        subjects: ["educationChildcare"]
    },
    {
        id: 22,
        name: "Havant and South Downs College",
        location: {
            fullAddress: "Havant Campus, New Road, Havant, Hampshire, PO9 1QL",
            postcode: "PO9 1QL",
            latitude: 50.858058,
            longitude: -0.986631
        },
        phoneNumber: "023 9387 9999",
        website: "https://www.hsdc.ac.uk/",
        subjects: ["digital", "construction", "educationChildcare"]
    },
    {
        id: 23,
        name: "HCUC - Harrow College",
        location: {
            fullAddress: "Lowlands Road, Harrow, Middlesex, HA1 3AQ",
            postcode: "HA1 3AQ",
            latitude: 51.577965,
            longitude: -0.335859
        },
        phoneNumber: "020 8909 6000",
        website: "https://www.hcuc.ac.uk/",
        subjects: ["digital", "educationChildcare"]
    },
    {
        id: 24,
        name: "La Retraite RC Girls School",
        location: {
            fullAddress: "Atkins Road, Clapham Park, London, SW12 0AB",
            postcode: "SW12 0AB",
            latitude: 51.44777,
            longitude: -0.143745
        },
        phoneNumber: "020 8673 5644",
        website: "https://www.laretraite.lambeth.sch.uk/",
        subjects: ["digital", "educationChildcare"]
    },
    {
        id: 25,
        name: "Lordswood Girls’ School & Sixth Form Centre",
        location: {
            fullAddress: "Knightlow Road, Harborne, Birmingham, B17 8QB",
            postcode: "B17 8QB",
            latitude: 52.468613,
            longitude: -1.962894
        },
        phoneNumber: "0121 429 2838",
        website: "www.lordswoodgirls.co.uk/",
        subjects: ["digital", "educationChildcare"]
    }
]
