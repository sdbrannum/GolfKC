// import { Coordinates } from "./types";
// /**
//  * Calculate Haversine distance between two points
//  * @remarks Credit https://stackoverflow.com/a/21623206/5508690
//  */
// export const haversine = (co1: Coordinates, co2: Coordinates) => {
//     const { lat: lat1, lon: lon1 } = co1;
//     const { lat: lat2, lon: lon2 } = co2;
//     const r = 6371; // km
//     const p = Math.PI / 180;
//    
//     const a = 0.5 - Math.cos((lat2 - lat1) * p) / 2
//                     + Math.cos(lat1 * p) * Math.cos(lat2 * p) *
//                     (1 - Math.cos((lon2 - lon1) * p)) / 2;
//    
//     return 2 * r * Math.asin(Math.sqrt(a));
// }