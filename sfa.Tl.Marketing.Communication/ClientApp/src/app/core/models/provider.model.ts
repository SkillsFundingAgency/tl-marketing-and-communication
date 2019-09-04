export interface IProvider {
    id: number;
    name: string;
    location: ILocation;
    phoneNumber: string;
    website: string;
    subjects: string[];
    isWithinDistance?: boolean;
}

export interface ILocation {
    fullAddress: string;
    postcode: string;
    longitude: number;
    latitude: number;
}
