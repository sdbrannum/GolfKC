declare global {
    type Course = {
        id: string;
        name: string;
        source: string;
        uri: string;
        address: Address;
        photo?: string;
    }
    
    type Address = {
        city: string;
        state: string;
    }
    
    type TeeTime = {
        rate: number;
        players: number;
        holes: number;
        time: string;
    }
}

export {};