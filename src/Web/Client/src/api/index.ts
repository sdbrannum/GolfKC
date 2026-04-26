export const getCourses = async () => {
    const res = await fetch('courses');
    return await res.json() as Course[];
}

/**
 * 
 * @param source
 * @param id
 * @param date format yyyy-mm-dd
 */
export const getTimes = async (source: string, id: string, date: string) => {
    const res = await fetch(`${source}/tee-times/${id}?date=${date}`);
    if (res.status !== 200) {
        throw new Error(`Error retrieving times for ${source} ${id}`);
    }
    return await res.json() as TeeTime[];
}
